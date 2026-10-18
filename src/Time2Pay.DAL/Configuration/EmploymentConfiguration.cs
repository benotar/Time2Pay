using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Time2Pay.DAL.Entities;

namespace Time2Pay.DAL.Configuration;

public class EmploymentConfiguration : IEntityTypeConfiguration<Employment>
{
    public void Configure(EntityTypeBuilder<Employment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasMaxLength(EntityLimits.IdMaxLength);

        builder.Property(e => e.Name).HasMaxLength(EntityLimits.NameMaxLength);

        builder.Property(e => e.Description).HasMaxLength(EntityLimits.DescriptionMaxLength);

        builder.ToTable(table => table.HasCheckConstraint(DalConstants.EmploymentsEndDateAfterStartDateConstraintName,
            DalConstants.EmploymentsEndDateAfterStartDateConstraint));
    }
}
