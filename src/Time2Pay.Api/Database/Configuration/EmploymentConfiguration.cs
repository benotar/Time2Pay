using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Time2Pay.Api.Entities;

namespace Time2Pay.Api.Database.Configuration;

public class EmploymentConfiguration : IEntityTypeConfiguration<Employment>
{
    public void Configure(EntityTypeBuilder<Employment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasMaxLength(Constants.IdMaxLength);

        builder.Property(e => e.Name).HasMaxLength(Constants.NameMaxLength);

        builder.Property(e => e.Description).HasMaxLength(Constants.DescriptionMaxLength);
    }
}
