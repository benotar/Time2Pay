using System.Linq.Expressions;
using Time2Pay.DAL.Entities;
using Time2Pay.DAL.Utils;
using Time2Pay.DTO.CompensationRates;

namespace Time2Pay.BLL.Mapping.CompensationRates;

public static class CompensationRateMapping
{
    private static readonly Expression<Func<CompensationRate, CompensationRateModel>> Projection = entity =>
        new CompensationRateModel
        {
            Id = entity.Id,
            EmploymentId = entity.EmploymentId,
            RateType = entity.RateType,
            Amount = entity.Amount,
            Currency = entity.Currency,
            ValidFromDate = entity.ValidFromDate,
            ValidToDate = entity.ValidToDate,
            CreatedAtUtc = entity.CreatedAtUtc,
            LastModifiedAtUtc = entity.LastModifiedAtUtc
        };

    private static readonly Func<CompensationRate, CompensationRateModel> CompiledProjection = Projection.Compile();

    public static Expression<Func<CompensationRate, CompensationRateModel>> ProjectToModel()
    {
        return Projection;
    }

    public static CompensationRateModel ToModel(this CompensationRate entity)
    {
        return CompiledProjection(entity);
    }

    public static CompensationRate ToEntity(this CreateCompensationRateModel model, string employmentId)
    {
        return new CompensationRate
        {
            Id = IdGenerator.PrefixId<CompensationRate>(),
            EmploymentId = employmentId,
            RateType = model.RateType,
            Amount = model.Amount,
            Currency = model.Currency,
            ValidFromDate = model.ValidFromDate,
            ValidToDate = model.ValidToDate
        };
    }
}
