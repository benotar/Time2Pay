using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Time2Pay.BLL.Errors;
using Time2Pay.BLL.Mapping.Employments;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DAL;
using Time2Pay.DAL.Providers.Interfaces;
using Time2Pay.DTO.Common;
using Time2Pay.DTO.Employments;

namespace Time2Pay.BLL.Services;

public sealed class EmploymentService : IEmploymentService
{
    private readonly IValidator<CreateEmploymentModel> _createValidator;
    private readonly IDateTimeOffsetProvider _dateTimeOffset;
    private readonly Time2PayDbContext _dbContext;
    private readonly IValidator<UpdateEmploymentModel> _updateValidator;

    public EmploymentService(IValidator<CreateEmploymentModel> createValidator, IDateTimeOffsetProvider dateTimeOffset,
        Time2PayDbContext dbContext, IValidator<UpdateEmploymentModel> updateValidator)
    {
        _dbContext = dbContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _dateTimeOffset = dateTimeOffset;
    }

    public async Task<Result<EmploymentModel>> GetByIdAsync(string userId, string id, CancellationToken cancellationToken)
    {
        var model = await _dbContext.Employments
            .AsNoTracking()
            .Where(emp => emp.UserId == userId && emp.Id == id)
            .Select(EmploymentMapping.ProjectToModel())
            .FirstOrDefaultAsync(cancellationToken);

        if (model is null)
        {
            return Result.Failure<EmploymentModel>(EmploymentErrors.NotFound(id));
        }

        return model;
    }

    public async Task<Result<PaginationResult<EmploymentModel>>> GetAsync(string userId, CommonQueryParameters query,
        CancellationToken cancellationToken)
    {
        var models = await _dbContext.Employments
            .AsNoTracking()
            .Where(emp => emp.UserId == userId)
            .WhereAnyContains(query.Search, emp => emp.Name, emp => emp.Description)
            .OrderBy(emp => emp.CreatedAtUtc)
            .Select(EmploymentMapping.ProjectToModel())
            .CreatePaginationResultAsync(query.Page, query.PageSize, cancellationToken);

        return models;
    }

    public async Task<Result<EmploymentModel>> CreateAsync(string userId, CreateEmploymentModel model,
        CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure<EmploymentModel>(ValidationViolation.FromResult(validationResult));
        }

        var entity = model.ToEntity(userId);

        _dbContext.Employments.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity.ToModel();
    }

    public async Task<Result> UpdateAsync(string userId, string id, UpdateEmploymentModel model,
        CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure(ValidationViolation.FromResult(validationResult));
        }

        var employment = await _dbContext.Employments.FirstOrDefaultAsync(emp => emp.UserId == userId && emp.Id == id,
            cancellationToken);

        if (employment is null)
        {
            return Result.Failure(EmploymentErrors.NotFound(id));
        }

        employment.UpdateFromModel(model);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(string userId, string id, CancellationToken cancellationToken)
    {
        var affectedRows = await _dbContext.Employments.Where(emp => emp.UserId == userId &&
                                                                     emp.Id == id && !emp.IsDeleted)
            .ExecuteUpdateAsync(x => x.SetProperty(emp => emp.IsDeleted, true)
                    .SetProperty(emp => emp.DeletedAtUtc, _dateTimeOffset.UtcNow),
                cancellationToken);

        return affectedRows == 0
            ? Result.Failure(EmploymentErrors.NotFound(id))
            : Result.Success();
    }
}
