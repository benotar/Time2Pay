using Time2Pay.BLL.Utils;
using Time2Pay.DTO.Employments;

namespace Time2Pay.BLL.Services.Interfaces;

public interface IEmploymentService
{
    Task<Result<EmploymentDto>> GetEmploymentAsync(string id, CancellationToken cancellationToken);

    Task<Result<EmploymentsCollectionDto>> GetEmploymentsAsync(CancellationToken cancellationToken);

    Task<Result<EmploymentDto>> CreateAsync(CreateEmploymentDto dto, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(string id, UpdateEmploymentDto dto, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken);
}
