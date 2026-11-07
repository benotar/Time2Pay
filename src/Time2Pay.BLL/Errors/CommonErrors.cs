using Time2Pay.BLL.Utils;

namespace Time2Pay.BLL.Errors;

public static class CommonErrors
{
    public static Error NotFound(string id, string entity)
    {
        return Error.NotFound($"{entity}.NotFound", $"The {entity} with id '{id}' was not found'");
    }
}
