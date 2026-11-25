using Time2Pay.BLL.Utils;
using Time2Pay.DTO.Common;
using Time2Pay.DTO.CompensationRates;

namespace Time2Pay.BLL.Services.Interfaces;

public interface ICompensationRateService
{
    Task<Result<CompensationRateModel>> GetByIdAsync(string userId, string employmentId, string id,
        CancellationToken cancellationToken);

    Task<Result<PaginationResult<CompensationRateModel>>> GetAsync(string userId, string employmentId,
        CommonQueryParameters query, CancellationToken cancellationToken);

    Task<Result<CompensationRateModel>> CreateAsync(string userId, string employmentId, CreateCompensationRateModel model,
        CancellationToken cancellationToken);
}
