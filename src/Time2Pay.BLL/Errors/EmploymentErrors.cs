using Time2Pay.BLL.Utils;

namespace Time2Pay.BLL.Errors;

public static class EmploymentErrors
{
    public static Error NotFound(string id)
    {
        return Error.NotFound("Employment.NotFound", $"The Employment with id '{id}' was not found");
    }
}
