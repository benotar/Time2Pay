namespace Time2Pay.Api.Entities;

public class Employment : AuditEntity
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public DateOnly StartedOn { get; set; }
    public DateOnly? EndedOn { get; set; }
}
