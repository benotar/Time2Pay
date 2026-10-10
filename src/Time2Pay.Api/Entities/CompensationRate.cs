using Time2Pay.Api.Entities.Enums;

namespace Time2Pay.Api.Entities;

public sealed class CompensationRate : AuditEntity
{
    //public string EmploymentId { get; set; }
    public RateType RateType { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public DateOnly ValidFromDate { get; set; }
    public DateOnly? ValidToDate { get; set; }
}
