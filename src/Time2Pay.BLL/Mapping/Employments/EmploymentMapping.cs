using System.Linq.Expressions;
using Time2Pay.DAL.Entities;
using Time2Pay.DAL.Utils;
using Time2Pay.DTO.Employments;

namespace Time2Pay.BLL.Mapping.Employments;

public static class EmploymentMapping
{
    private static readonly Expression<Func<Employment, EmploymentModel>> Projection = employment => new EmploymentModel
    {
        Id = employment.Id,
        Name = employment.Name,
        Description = employment.Description,
        StartDate = employment.StartDate,
        EndDate = employment.EndDate,
        CreatedAtUtc = employment.CreatedAtUtc,
        LastModifiedAtUtc = employment.LastModifiedAtUtc
    };

    private static readonly Func<Employment, EmploymentModel> CompiledProjection = Projection.Compile();

    public static Expression<Func<Employment, EmploymentModel>> ProjectToModel()
    {
        return Projection;
    }

    public static EmploymentModel ToModel(this Employment entity)
    {
        return CompiledProjection(entity);
    }

    public static Employment ToEntity(this CreateEmploymentModel model)
    {
        return new Employment
        {
            Id = IdGenerator.PrefixId<Employment>(),
            Name = model.Name,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate
        };
    }

    public static void UpdateFromModel(this Employment employment, UpdateEmploymentModel model)
    {
        // employment.LastModifiedAtUtc is going to be updated in DAL.UpdateAuditEntityInterceptor

        employment.Name = model.Name;
        employment.Description = model.Description;
        employment.StartDate = model.StartDate;
        employment.EndDate = model.EndDate;
    }
}
