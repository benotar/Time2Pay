namespace Time2Pay.DTO.Employments;

public sealed record CreateEmploymentDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
}
