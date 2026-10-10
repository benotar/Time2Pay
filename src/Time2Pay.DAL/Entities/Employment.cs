namespace Time2Pay.DAL.Entities;

public sealed class Employment : AuditEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
