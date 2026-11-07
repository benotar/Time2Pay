using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Time2Pay.BLL.Errors;
using Time2Pay.BLL.Mapping.WorkDays;
using Time2Pay.BLL.Services.Interfaces;
using Time2Pay.BLL.Utils;
using Time2Pay.DAL;
using Time2Pay.DTO.WorkDay;

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

    // TODO Add user validation. Return data only by current user
    public async Task<Result<WorkDayModel>> GetWorkDayAsync(string id, CancellationToken cancellationToken)
    {
        var model = await _dbContext.WorkDays
            .AsNoTracking()
            .Where(wd => wd.Id == id)
            .Select(WorkDayMapping.ProjectToModel())
            .FirstOrDefaultAsync(cancellationToken);

        if (model is null)
        {
            return Result.Failure<WorkDayModel>(WorkDayErrors.NotFound(id));
        }

        return model;
    }

    // TODO Make it idempotent for the user. One user can have only one workday for the respective employment
    public async Task<Result<WorkDayModel>> CreateAsync(CreateWorkDayModel model, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure<WorkDayModel>(ValidationViolation.FromResult(validationResult));
        }

        var isEmploymentExists = await _dbContext.Employments.AnyAsync(emp => model.EmploymentId == emp.Id, cancellationToken);

        if (!isEmploymentExists)
        {
            return Result.Failure<WorkDayModel>(EmploymentErrors.NotFound(model.EmploymentId));
        }

        var entity = model.ToEntity();

        _dbContext.WorkDays.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity.ToModel();
    }
}
