namespace Time2Pay.DAL.Entities;

public abstract class AuditEntity
{
    public string Id { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? LastModifiedAtUtc { get; set; }
}
