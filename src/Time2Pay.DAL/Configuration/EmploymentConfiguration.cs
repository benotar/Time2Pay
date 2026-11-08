using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Time2Pay.DAL.Entities;
using Time2Pay.Shared;

namespace Time2Pay.DAL.Configuration;

public class EmploymentConfiguration : IEntityTypeConfiguration<Employment>
{
    public void Configure(EntityTypeBuilder<Employment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasMaxLength(Constants.IdMaxLength);

        builder.Property(e => e.Name).HasMaxLength(Constants.NameMaxLength);

        builder.Property(e => e.Description).HasMaxLength(Constants.DescriptionMaxLength);

        builder.ToTable(table => table.HasCheckConstraint(Constants.EmploymentsEndDateAfterStartDateConstraintName,
            Constants.EmploymentsEndDateAfterStartDateConstraint));
    }
}
