using Time2Pay.BLL.Utils;
using Time2Pay.DAL.Entities;

namespace Time2Pay.BLL.Errors;

public static class WorkDayErrors
{
    public static Error NotFound(string id)
    {
        return CommonErrors.NotFound(id, nameof(WorkDay));
    }

    public static Error Conflict(string userId, string employmentId, DateOnly date)
    {
        return Error.Conflict($"{nameof(WorkDay)}.{nameof(ErrorType.Conflict)}",
            $"The user [{userId}] already has the record for the [{nameof(Employment)}].[{employmentId}] on {date}");
    }
}
