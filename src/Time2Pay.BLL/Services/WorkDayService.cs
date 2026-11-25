using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Time2Pay.BLL.Errors;
using Time2Pay.BLL.Mapping.WorkDays;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DAL;
using Time2Pay.DTO.Common;
using Time2Pay.DTO.WorkDays;

namespace Time2Pay.BLL.Services;

public class WorkDayService : IWorkDayService
{
    private readonly IValidator<CreateWorkDayModel> _createValidator;
    private readonly Time2PayDbContext _dbContext;

    public WorkDayService(Time2PayDbContext dbContext, IValidator<CreateWorkDayModel> createValidator)
    {
        _dbContext = dbContext;
        _createValidator = createValidator;
    }

    public async Task<Result<WorkDayModel>> GetByIdAsync(string userId, string employmentId, string id,
        CancellationToken cancellationToken)
    {
        var model = await _dbContext.WorkDays
            .AsNoTracking()
            .Where(day => day.Employment.UserId == userId)
            .Where(day => day.EmploymentId == employmentId)
            .Where(day => day.Id == id)
            .Select(WorkDayMapping.ProjectToModel())
            .FirstOrDefaultAsync(cancellationToken);

        if (model is null)
        {
            return Result.Failure<WorkDayModel>(WorkDayErrors.NotFound(id));
        }

        return model;
    }

    public async Task<Result<PaginationResult<WorkDayModel>>> GetAsync(string userId, string employmentId,
        CommonQueryParameters query,
        CancellationToken cancellationToken)
    {
        var models = await _dbContext.WorkDays
            .AsNoTracking()
            .Where(day => day.Employment.UserId == userId && day.Employment.Id == employmentId)
            .OrderBy(day => day.CreatedAtUtc)
            .Select(WorkDayMapping.ProjectToModel())
            .CreatePaginationResultAsync(query.Page, query.PageSize, cancellationToken);

        return models;
    }

    public async Task<Result<WorkDayModel>> CreateAsync(string userId, string employmentId, CreateWorkDayModel model,
        CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure<WorkDayModel>(ValidationViolation.FromResult(validationResult));
        }

        var employment = await _dbContext.Employments
            .Where(emp => emp.Id == employmentId)
            .Select(emp => new
            {
                EmployeeId = emp.UserId,
                HasWorkDay = emp.WorkDays.Any(wd => wd.Date == model.Date)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (employment is null)
        {
            return Result.Failure<WorkDayModel>(EmploymentErrors.NotFound(employmentId));
        }

        if (employment.EmployeeId != userId)
        {
            return Result.Failure<WorkDayModel>(EmploymentErrors.Forbidden(userId, employmentId));
        }

        if (employment.HasWorkDay)
        {
            return Result.Failure<WorkDayModel>(WorkDayErrors.Conflict(userId, employmentId, model.Date));
        }

        var entity = model.ToEntity(employmentId);

        _dbContext.WorkDays.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity.ToModel();
    }
}
