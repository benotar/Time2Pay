using System.Linq.Expressions;
using Time2Pay.DAL.Entities;
using Time2Pay.DAL.Utils;
using Time2Pay.DTO.WorkDay;

namespace Time2Pay.BLL.Mapping.WorkDays;

public static class WorkDayMapping
{
    private static readonly Expression<Func<WorkDay, WorkDayModel>> Projection = entity => new WorkDayModel
    {
        Id = entity.Id,
        EmploymentId = entity.EmploymentId,
        Date = entity.Date,
        DayType = entity.DayType,
        StartedAt = entity.StartedAt,
        EndedAt = entity.EndedAt,
        BreakMinutes = entity.BreakMinutes,
        DurationMinutes = entity.DurationMinutes,
        Comment = entity.Comment,
        CreatedAtUtc = entity.CreatedAtUtc,
        LastModifiedAtUtc = entity.LastModifiedAtUtc
    };

    private static readonly Func<WorkDay, WorkDayModel> CompiledProjection = Projection.Compile();

    public static Expression<Func<WorkDay, WorkDayModel>> ProjectToModel()
    {
        return Projection;
    }

    public static WorkDayModel ToModel(this WorkDay entity)
    {
        return CompiledProjection(entity);
    }

    public static WorkDay ToEntity(this CreateWorkDayModel model)
    {
        return new WorkDay
        {
            Id = IdGenerator.PrefixId<WorkDay>(),
            EmploymentId = model.EmploymentId,
            Date = model.Date,
            DayType = model.DayType,
            StartedAt = model.StartedAt,
            EndedAt = model.EndedAt,
            BreakMinutes = model.BreakMinutes,
            Comment = model.Comment
        };
    }
}
