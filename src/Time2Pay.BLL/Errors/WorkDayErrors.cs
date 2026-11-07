using Time2Pay.BLL.Utils;
using Time2Pay.DAL.Entities;

namespace Time2Pay.BLL.Errors;

public static class WorkDayErrors
{
    public static Error NotFound(string id)
    {
        return CommonErrors.NotFound(id, nameof(WorkDay));
    }
}
