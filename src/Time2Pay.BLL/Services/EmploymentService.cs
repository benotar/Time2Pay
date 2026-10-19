using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Time2Pay.BLL.Errors;
using Time2Pay.BLL.Mapping.Employments;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DAL;
using Time2Pay.DTO.Employments;

namespace Time2Pay.BLL.Services;

public sealed class EmploymentService : IEmploymentService
{
    private readonly Time2PayDbContext _dbContext;
    private readonly IValidator<CreateEmploymentDto> _createValidator;
    private readonly IValidator<UpdateEmploymentDto> _updateValidator;

    public EmploymentService(Time2PayDbContext dbContext, IValidator<CreateEmploymentDto> createValidator,
        IValidator<UpdateEmploymentDto> updateValidator)
    {
        _dbContext = dbContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<EmploymentDto>> GetEmploymentAsync(string id, CancellationToken cancellationToken)
    {
        var employment = await _dbContext.Employments
            .AsNoTracking()
            .Where(emp => emp.Id == id)
            .Select(EmploymentMapping.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (employment is null)
        {
            return Result.Failure<EmploymentDto>(EmploymentErrors.NotFound(id));
        }

        return employment;
    }

    public async Task<Result<EmploymentsCollectionDto>> GetEmploymentsAsync(EmploymentsQueryParameters query,
        CancellationToken cancellationToken)
    {
        var employmentDtoList = await _dbContext.Employments
            .AsNoTracking()
            .WhereAnyContains(query.Search, emp => emp.Name, emp => emp.Description)
            .Select(EmploymentMapping.ProjectToDto())
            .ToListAsync(cancellationToken);

        return new EmploymentsCollectionDto
        {
            Items = employmentDtoList
        };
    }

    public async Task<Result<EmploymentDto>> CreateAsync(CreateEmploymentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure<EmploymentDto>(ValidationViolation.FromResult(validationResult));
        }

        var employment = dto.ToEntity();

        _dbContext.Employments.Add(employment);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return employment.ToDto();
    }

    public async Task<Result> UpdateAsync(string id, UpdateEmploymentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure(ValidationViolation.FromResult(validationResult));
        }

        var employment = await _dbContext.Employments.FirstOrDefaultAsync(emp => emp.Id == id, cancellationToken);

        if (employment is null)
        {
            return Result.Failure(EmploymentErrors.NotFound(id));
        }

        employment.UpdateFromDto(dto);

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
