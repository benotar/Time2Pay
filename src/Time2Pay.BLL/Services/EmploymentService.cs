using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Time2Pay.BLL.Errors;
using Time2Pay.BLL.Mapping.Employments;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DAL;
using Time2Pay.DTO.Common;
using Time2Pay.DTO.Employments;

namespace Time2Pay.BLL.Services;

public sealed class EmploymentService : IEmploymentService
{
    private readonly Time2PayDbContext _dbContext;
    private readonly IValidator<CreateEmploymentModel> _createValidator;
    private readonly IValidator<UpdateEmploymentModel> _updateValidator;

    public EmploymentService(Time2PayDbContext dbContext, IValidator<CreateEmploymentModel> createValidator,
        IValidator<UpdateEmploymentModel> updateValidator)
    {
        _dbContext = dbContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<EmploymentModel>> GetEmploymentAsync(string id, CancellationToken cancellationToken)
    {
        var employment = await _dbContext.Employments
            .AsNoTracking()
            .Where(emp => emp.Id == id)
            .Select(EmploymentMapping.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (employment is null)
        {
            return Result.Failure<EmploymentModel>(EmploymentErrors.NotFound(id));
        }

        return employment;
    }

    public async Task<Result<PaginationResult<EmploymentModel>>> GetEmploymentsAsync(EmploymentsQueryParameters query,
        CancellationToken cancellationToken)
    {
        var employments = await _dbContext.Employments
            .AsNoTracking()
            .WhereAnyContains(query.Search, emp => emp.Name, emp => emp.Description)
            .OrderBy(emp => emp.CreatedAtUtc)
            .Select(EmploymentMapping.ProjectToDto())
            .CreatePaginationResultAsync(query.Page, query.PageSize, cancellationToken);

        return employments;
    }

    public async Task<Result<EmploymentModel>> CreateAsync(CreateEmploymentModel model, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure<EmploymentModel>(ValidationViolation.FromResult(validationResult));
        }

        var employment = model.ToEntity();

        _dbContext.Employments.Add(employment);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return employment.ToDto();
    }

    public async Task<Result> UpdateAsync(string id, UpdateEmploymentModel model, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure(ValidationViolation.FromResult(validationResult));
        }

        var employment = await _dbContext.Employments.FirstOrDefaultAsync(emp => emp.Id == id, cancellationToken);

        if (employment is null)
        {
            return Result.Failure(EmploymentErrors.NotFound(id));
        }

        employment.UpdateFromModel(model);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var employment = await _dbContext.Employments.FirstOrDefaultAsync(emp => emp.Id == id, cancellationToken);

        if (employment is null)
        {
            return Result.Failure(EmploymentErrors.NotFound(id));
        }

        _dbContext.Employments.Remove(employment);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
