using Time2Pay.Api.Entities.Enums;

namespace Time2Pay.Api.Entities;

public sealed class Payment : AuditEntity
{
    //public string EmploymentId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string Comment { get; set; }
    public DateOnly PaidDate { get; set; }
    public DateOnly PeriodStartDate { get; set; }
    public DateOnly PeriodEndDate { get; set; }
}
