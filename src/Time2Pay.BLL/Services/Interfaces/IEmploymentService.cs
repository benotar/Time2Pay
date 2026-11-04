using Time2Pay.BLL.Utils;
using Time2Pay.DTO.Common;
using Time2Pay.DTO.Employments;

namespace Time2Pay.BLL.Services.Interfaces;

public interface IEmploymentService
{
    Task<Result<EmploymentModel>> GetEmploymentAsync(string id, CancellationToken cancellationToken);

    Task<Result<PaginationResult<EmploymentModel>>> GetEmploymentsAsync(EmploymentsQueryParameters query,
        CancellationToken cancellationToken);

    Task<Result<EmploymentModel>> CreateAsync(CreateEmploymentModel model, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(string id, UpdateEmploymentModel model, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken);
}
