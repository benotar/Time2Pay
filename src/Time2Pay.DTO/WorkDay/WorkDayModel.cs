using Time2Pay.Shared.Enums;

namespace Time2Pay.DTO.WorkDay;

public sealed record WorkDayModel
{
    public required string Id { get; init; }
    public required string EmploymentId { get; init; }
    public required DateOnly Date { get; init; }
    public required DayType DayType { get; init; }
    public TimeOnly? StartedAt { get; init; }
    public TimeOnly? EndedAt { get; init; }
    public required int BreakMinutes { get; init; }
    public int? DurationMinutes { get; set; }
    public string? Comment { get; init; }
    public required DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? LastModifiedAtUtc { get; init; }
}
