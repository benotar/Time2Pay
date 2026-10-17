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
            statusCode: GetStatusCode(result.Error.Type),
            extensions: GetErrors(result));

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
