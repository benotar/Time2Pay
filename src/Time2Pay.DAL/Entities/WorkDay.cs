using Time2Pay.DAL.Enums;

namespace Time2Pay.DAL.Entities;

public sealed class WorkDay : AuditEntity, IPrefix
{
    public string EmploymentId { get; set; }
    public DayType DayType { get; set; }
    public int BreakMinutes { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Comment { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly? StartedAt { get; set; }
    public TimeOnly? EndedAt { get; set; }

    public static string Prefix => "wday_";
}
