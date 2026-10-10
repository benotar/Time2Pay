using Time2Pay.DAL.Enums;

namespace Time2Pay.DAL.Entities;

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
