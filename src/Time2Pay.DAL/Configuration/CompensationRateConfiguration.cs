using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Time2Pay.DAL.Entities;
using Time2Pay.Shared;

namespace Time2Pay.DAL.Configuration;

public class CompensationRateConfiguration : IEntityTypeConfiguration<CompensationRate>
{
    public void Configure(EntityTypeBuilder<CompensationRate> builder)
    {
        builder.HasKey(rate => rate.Id);

        builder.Property(rate => rate.Id).HasMaxLength(Constants.IdMaxLength);

        builder.Property(rate => rate.Amount).HasPrecision(12, 2);

        builder.Property(rate => rate.RateType).HasConversion<string>();

        builder.Property(rate => rate.Currency).HasConversion<string>();

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(Constants.CompensationRatesRateTypeIsValidConstraintName,
                Constants.CompensationRatesRateTypeIsValidConstraint);

            table.HasCheckConstraint(Constants.CompensationRatesCurrencyIsValidConstraintName,
                Constants.CompensationRatesCurrencyIsValidConstraint);
        });

        builder.HasOne<Employment>()
            .WithMany()
            .HasForeignKey(rate => rate.EmploymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
