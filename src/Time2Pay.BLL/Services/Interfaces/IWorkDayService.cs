using Time2Pay.BLL.Utils;
using Time2Pay.DTO.WorkDay;

namespace Time2Pay.BLL.Services.Interfaces;

public interface IWorkDayService
{
    Task<Result<WorkDayModel>> GetWorkDayAsync(string id, CancellationToken cancellationToken);

    Task<Result<WorkDayModel>> CreateAsync(string userId, CreateWorkDayModel model, CancellationToken cancellationToken);
}
