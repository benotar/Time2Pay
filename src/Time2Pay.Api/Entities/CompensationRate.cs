using Time2Pay.Api.Entities.Enums;

namespace Time2Pay.Api.Entities;

public class CompensationRate : AuditEntity
{
    public string EmploymentId { get; set; }
    public RateType RateType { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
}
