using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Time2Pay.DAL.Entities;
using Time2Pay.Shared;

namespace Time2Pay.DAL.Configuration;

public class WorkDayConfiguration : IEntityTypeConfiguration<WorkDay>
{
    public void Configure(EntityTypeBuilder<WorkDay> builder)
    {
        builder.HasKey(day => day.Id);

        builder.Property(day => day.Id).HasMaxLength(Constants.IdMaxLength);

        builder.Property(day => day.Comment).HasMaxLength(Constants.CommentMaxLength);

        builder.Property(day => day.DurationMinutes)
            .HasComputedColumnSql(Constants.DurationMinutesComputedColumnSql, true);

        builder.HasIndex(day => new { day.EmploymentId, day.Date })
            .IsUnique();

        builder.Property(day => day.DayType).HasConversion<string>();

        builder.HasQueryFilter(day => !day.Employment.IsDeleted);

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

        builder.HasOne<Employment>(day => day.Employment)
            .WithMany(emp => emp.WorkDays)
            .HasForeignKey(day => day.EmploymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
