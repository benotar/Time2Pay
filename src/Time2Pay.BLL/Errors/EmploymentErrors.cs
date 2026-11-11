using Time2Pay.BLL.Utils;
using Time2Pay.DAL.Entities;

namespace Time2Pay.BLL.Errors;

public static class EmploymentErrors
{
    public static Error NotFound(string id)
    {
        return CommonErrors.NotFound(id, nameof(Employment));
    }

    public static Error Forbidden(string userId, string entityId)
    {
        return CommonErrors.Forbidden(userId, entityId, nameof(Employment));
    }
}
