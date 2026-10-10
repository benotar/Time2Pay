using Time2Pay.DTO.Employments;

namespace Time2Pay.BLL.Services.Interfaces;

public interface IEmploymentService
{
    Task<EmploymentDto?> GetEmploymentAsync(string id, CancellationToken cancellationToken);

    Task<EmploymentsCollectionDto> GetEmploymentsAsync(CancellationToken cancellationToken);

    Task<EmploymentDto> CreateAsync(CreateEmploymentDto dto, CancellationToken cancellationToken);
}
