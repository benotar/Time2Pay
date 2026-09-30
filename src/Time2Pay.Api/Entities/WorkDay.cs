using Time2Pay.Api.Entities.Enums;

namespace Time2Pay.Api.Entities;

public class WorkDay : AuditEntity
{
    public string EmploymentId { get; set; }
    public DateOnly Date { get; set; }
    public DayType DayType { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public int BreakMinutes { get; set; }
    public int DurationMinutes { get; set; }
    public string Comment { get; set; }
}
