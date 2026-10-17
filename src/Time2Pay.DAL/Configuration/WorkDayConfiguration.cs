using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Time2Pay.DAL.Entities;

namespace Time2Pay.DAL.Configuration;

public class WorkDayConfiguration : IEntityTypeConfiguration<WorkDay>
{
    public void Configure(EntityTypeBuilder<WorkDay> builder)
    {
        builder.HasKey(wd => wd.Id);

        builder.Property(wd => wd.Id).HasMaxLength(DalConstants.IdMaxLength);

        builder.Property(wd => wd.Comment).HasMaxLength(DalConstants.CommentMaxLength);

        builder.Property(wd => wd.DurationMinutes)
            .HasComputedColumnSql(DalConstants.DurationMinutesComputedColumnSql, true);

        builder.HasIndex(wd => new { wd.EmploymentId, wd.Date })
            .IsUnique();

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(DalConstants.WorkDaysDayTypeIsValidConstraintName,
                DalConstants.WorkDaysDayTypeIsValidConstraint);

            table.HasCheckConstraint(DalConstants.WorkDaysBreakMinutesConstraintName,
                DalConstants.WorkDaysBreakMinutesConstraint);

            table.HasCheckConstraint(DalConstants.WorkDaysTimesMatchDayTypeConstraintName,
                DalConstants.WorkDaysTimesMatchDayTypeConstraint);

            table.HasCheckConstraint(DalConstants.WorkDaysEndedAfterStartedConstraintName,
                DalConstants.WorkDaysEndedAfterStartedConstraint);

            table.HasCheckConstraint(DalConstants.WorkDaysBreakWithinShiftConstraintName,
                DalConstants.WorkDaysBreakWithinShiftConstraint);
        });

        builder.HasOne<Employment>()
            .WithMany()
            .HasForeignKey(wd => wd.EmploymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
