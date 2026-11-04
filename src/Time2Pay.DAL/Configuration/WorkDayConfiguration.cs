using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Time2Pay.DAL.Entities;
using Time2Pay.Shared;

namespace Time2Pay.DAL.Configuration;

public class WorkDayConfiguration : IEntityTypeConfiguration<WorkDay>
{
    public void Configure(EntityTypeBuilder<WorkDay> builder)
    {
        builder.HasKey(wd => wd.Id);

        builder.Property(wd => wd.Id).HasMaxLength(Constants.IdMaxLength);

        builder.Property(wd => wd.Comment).HasMaxLength(Constants.CommentMaxLength);

        builder.Property(wd => wd.DurationMinutes)
            .HasComputedColumnSql(Constants.DurationMinutesComputedColumnSql, true);

        builder.HasIndex(wd => new { wd.EmploymentId, wd.Date })
            .IsUnique();

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(Constants.WorkDaysDayTypeIsValidConstraintName,
                Constants.WorkDaysDayTypeIsValidConstraint);

            table.HasCheckConstraint(Constants.WorkDaysBreakMinutesConstraintName,
                Constants.WorkDaysBreakMinutesConstraint);

            table.HasCheckConstraint(Constants.WorkDaysTimesMatchDayTypeConstraintName,
                Constants.WorkDaysTimesMatchDayTypeConstraint);

            table.HasCheckConstraint(Constants.WorkDaysEndedAfterStartedConstraintName,
                Constants.WorkDaysEndedAfterStartedConstraint);

            table.HasCheckConstraint(Constants.WorkDaysBreakWithinShiftConstraintName,
                Constants.WorkDaysBreakWithinShiftConstraint);
        });

        builder.HasOne<Employment>()
            .WithMany()
            .HasForeignKey(wd => wd.EmploymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
