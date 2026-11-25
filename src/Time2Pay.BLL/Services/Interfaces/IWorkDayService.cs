using Time2Pay.BLL.Utils;
using Time2Pay.DTO.Common;
using Time2Pay.DTO.WorkDays;

namespace Time2Pay.BLL.Services.Interfaces;

public interface IWorkDayService
{
    Task<Result<WorkDayModel>> GetByIdAsync(string userId, string employmentId, string id,
        CancellationToken cancellationToken);

    Task<Result<PaginationResult<WorkDayModel>>> GetAsync(string userId, string employmentId, CommonQueryParameters query,
        CancellationToken cancellationToken);

    Task<Result<WorkDayModel>> CreateAsync(string userId, string employmentId, CreateWorkDayModel model,
        CancellationToken cancellationToken);
}
