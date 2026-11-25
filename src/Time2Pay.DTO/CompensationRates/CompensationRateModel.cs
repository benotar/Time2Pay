using Time2Pay.Shared.Enums;

namespace Time2Pay.DTO.CompensationRates;

public sealed record CompensationRateModel
{
    public required string Id { get; init; }
    public required string EmploymentId { get; init; }
    public required RateType RateType { get; init; }
    public required decimal Amount { get; init; }
    public required Currency Currency { get; init; }
    public required DateOnly ValidFromDate { get; init; }
    public DateOnly? ValidToDate { get; init; }
    public required DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? LastModifiedAtUtc { get; init; }
}
