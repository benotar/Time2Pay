using Time2Pay.Shared.Enums;

namespace Time2Pay.DTO.CompensationRates;

public sealed record CreateCompensationRateModel
{
    public required RateType RateType { get; init; }
    public required decimal Amount { get; init; }
    public required Currency Currency { get; init; }
    public required DateOnly ValidFromDate { get; init; }
    public DateOnly? ValidToDate { get; init; }
}
