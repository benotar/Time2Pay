using System.Linq.Expressions;
using Time2Pay.DAL.Entities;
using Time2Pay.DAL.Utils;
using Time2Pay.DTO.Employments;

namespace Time2Pay.BLL.Mapping.Employments;

public static class EmploymentMapping
{
    private static readonly Expression<Func<Employment, EmploymentDto>> Projection = employment => new EmploymentDto
    {
        Id = employment.Id,
        Name = employment.Name,
        Description = employment.Description,
        StartDate = employment.StartDate,
        EndDate = employment.EndDate,
        CreatedAtUtc = employment.CreatedAtUtc,
        LastModifiedAtUtc = employment.LastModifiedAtUtc
    };

    private static readonly Func<Employment, EmploymentDto> CompiledProjection = Projection.Compile();

    public static Expression<Func<Employment, EmploymentDto>> ProjectToDto()
    {
        return Projection;
    }

    public static EmploymentDto ToDto(this Employment employment)
    {
        return CompiledProjection(employment);
    }

    public static Employment ToEntity(this CreateEmploymentDto dto)
    {
        return new Employment
        {
            Id = IdGenerator.PrefixId<Employment>(),
            Name = dto.Name,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
    }
}
