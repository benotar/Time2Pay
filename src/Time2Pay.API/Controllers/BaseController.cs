using Microsoft.AspNetCore.Mvc;
using Time2Pay.BLL.Utils;

namespace Time2Pay.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected ActionResult ToProblem(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create a problem from a successful result");
        }

        var isFailure = result.Error.Type is ErrorType.Failure;

        var title = isFailure
            ? "Internal Server Error"
            : result.Error.Code;

        var detail = isFailure
            ? "An unexpected error occurred"
            : result.Error.Description;

        return Problem(title: title,
            detail: detail,
            type: GetErrorType(result.Error.Type),
            statusCode: GetStatusCode(result.Error.Type),
            extensions: GetErrors(result));

        static string GetErrorType(ErrorType errorType) => errorType switch
        {
            ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

        static int GetStatusCode(ErrorType errorType) => errorType switch
        {
            ErrorType.Validation or ErrorType.Problem => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        static Dictionary<string, object?>? GetErrors(Result result)
        {
            if (result.Error is not ValidationError validationError)
            {
                return null;
            }

            return new Dictionary<string, object?>
            {
                { "errors", validationError.Errors }
            };
        }
    }
}
