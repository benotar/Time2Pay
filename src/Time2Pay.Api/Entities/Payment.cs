using Time2Pay.Api.Entities.Enums;

namespace Time2Pay.Api.Entities;

public class Payment : AuditEntity
{
    public string EmploymentId { get; set; }
    public DateOnly PaidOn { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DateTimeOffset PeriodStartUtc { get; set; }
    public DateTimeOffset PeriodEndUtc { get; set; }
    public string Comment { get; set; }
}
