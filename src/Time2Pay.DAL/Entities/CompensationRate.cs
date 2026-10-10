using Time2Pay.DAL.Enums;

namespace Time2Pay.DAL.Entities;

public sealed class CompensationRate : AuditEntity
{
    //public string EmploymentId { get; set; }
    public RateType RateType { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public DateOnly ValidFromDate { get; set; }
    public DateOnly? ValidToDate { get; set; }
}
