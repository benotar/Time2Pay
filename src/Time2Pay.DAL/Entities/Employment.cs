namespace Time2Pay.DAL.Entities;

public sealed class Employment : AuditEntity, IPrefix, ISoftDelete
{
    public string UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public ICollection<WorkDay> WorkDays { get; set; } = [];

    public static string Prefix => "emp_";
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAtUtc { get; set; }
}
