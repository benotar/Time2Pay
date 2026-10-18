namespace Time2Pay.DAL;

internal static class DalConstants
{
    // Database

    public const string ApplicationSchema = "time_2_pay";

    public const string DatabaseSection = "Database";

    public const string WorkDayShiftTime = "(EXTRACT(EPOCH FROM (ended_at - started_at)) / 60)";
    public const string DurationMinutesComputedColumnSql = $"{WorkDayShiftTime}::int - break_minutes";

    // Employments constraints

    public const string EmploymentsEndDateAfterStartDateConstraintName = "ck_employments_end_date_after_start_date";

    // SQL CHECK allows true and NULL, but doesn't false.
    // If end_date is null the expression end_date >= start_date returns NULL, not false, and it will be passed to the table.
    // Explicit rule helps those who are unaware of such behavior.
    public const string EmploymentsEndDateAfterStartDateConstraint = "end_date IS NULL OR end_date >= start_date";


    // WorkDays constraints

    public const string WorkDaysDayTypeIsValidConstraintName = "ck_work_days_day_type_is_valid";
    public const string WorkDaysDayTypeIsValidConstraint = "day_type BETWEEN 1 AND 4";

    public const string WorkDaysBreakMinutesConstraintName = "ck_work_days_break_minutes_non_negative";
    public const string WorkDaysBreakMinutesConstraint = "break_minutes >= 0";

    public const string WorkDaysTimesMatchDayTypeConstraintName = "ck_work_days_times_match_day_type";

    public const string WorkDaysTimesMatchDayTypeConstraint = """
                                                              (day_type = 1 AND started_at IS NOT NULL AND ended_at IS NOT NULL) 
                                                              OR (day_type <> 1 AND started_at IS NULL AND ended_at IS NULL)
                                                              """;

    public const string WorkDaysEndedAfterStartedConstraintName = "ck_work_days_ended_after_started";
    public const string WorkDaysEndedAfterStartedConstraint = "ended_at > started_at";

    public const string WorkDaysBreakWithinShiftConstraintName = "ck_work_days_break_within_shift";
    public const string WorkDaysBreakWithinShiftConstraint = $"break_minutes <= {WorkDayShiftTime}";
}
