using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Time2Pay.BLL.Errors;
using Time2Pay.BLL.Mapping.CompensationRates;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DAL;
using Time2Pay.DTO.Common;
using Time2Pay.DTO.CompensationRates;

namespace Time2Pay.BLL.Services;

public class CompensationRateService : ICompensationRateService
{
    private readonly IValidator<CreateCompensationRateModel> _createValidator;
    private readonly Time2PayDbContext _dbContext;

    public CompensationRateService(Time2PayDbContext dbContext, IValidator<CreateCompensationRateModel> createValidator)
    {
        _dbContext = dbContext;
        _createValidator = createValidator;
    }

    public async Task<Result<CompensationRateModel>> GetByIdAsync(string userId, string employmentId, string id,
        CancellationToken cancellationToken)
    {
        var model = await _dbContext.CompensationRates
            .AsNoTracking()
            .Where(rate => rate.Employment.UserId == userId)
            .Where(rate => rate.EmploymentId == employmentId)
            .Where(rate => rate.Id == id)
            .Select(CompensationRateMapping.ProjectToModel())
            .FirstOrDefaultAsync(cancellationToken);

        if (model is null)
        {
            return Result.Failure<CompensationRateModel>(CompensationRateErrors.NotFound(id));
        }

        return model;
    }

    public async Task<Result<PaginationResult<CompensationRateModel>>> GetAsync(string userId, string employmentId,
        CommonQueryParameters query, CancellationToken cancellationToken)
    {
        var models = await _dbContext.CompensationRates
            .AsNoTracking()
            .Where(rate => rate.Employment.UserId == userId && rate.EmploymentId == employmentId)
            .OrderBy(rate => rate.CreatedAtUtc)
            .Select(CompensationRateMapping.ProjectToModel())
            .CreatePaginationResultAsync(query.Page, query.PageSize, cancellationToken);

        return models;
    }

    public async Task<Result<CompensationRateModel>> CreateAsync(string userId, string employmentId,
        CreateCompensationRateModel model, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure<CompensationRateModel>(ValidationViolation.FromResult(validationResult));
        }

        var employment = await _dbContext.Employments
            .FirstOrDefaultAsync(emp => emp.Id == employmentId, cancellationToken);

        if (employment is null)
        {
            return Result.Failure<CompensationRateModel>(EmploymentErrors.NotFound(employmentId));
        }

        if (employment.UserId != userId)
        {
            return Result.Failure<CompensationRateModel>(EmploymentErrors.Forbidden(userId, employmentId));
        }

        // TODO Add rates overlap check

        var entity = model.ToEntity(employmentId);

        _dbContext.CompensationRates.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity.ToModel();
    }
}
