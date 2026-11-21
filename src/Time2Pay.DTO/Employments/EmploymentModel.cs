namespace Time2Pay.DTO.Employments;

// TODO Do I need UserId here?
public sealed record EmploymentModel : IEmploymentModel
{
    public required string Id { get; init; }
    public required string UserId { get; init; }
    public required DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? LastModifiedAtUtc { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
}
