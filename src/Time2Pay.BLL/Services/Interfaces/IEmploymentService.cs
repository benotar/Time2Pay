using Time2Pay.BLL.Utils;
using Time2Pay.DTO.Common;
using Time2Pay.DTO.Employments;

namespace Time2Pay.BLL.Services.Interfaces;

public interface IEmploymentService
{
    Task<Result<EmploymentModel>> GetByIdAsync(string userId, string id, CancellationToken cancellationToken);

    Task<Result<PaginationResult<EmploymentModel>>> GetAsync(string userId, CommonQueryParameters query,
        CancellationToken cancellationToken);

    Task<Result<EmploymentModel>> CreateAsync(string userId, CreateEmploymentModel model, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(string userId, string id, UpdateEmploymentModel model, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(string userId, string id, CancellationToken cancellationToken);
}
