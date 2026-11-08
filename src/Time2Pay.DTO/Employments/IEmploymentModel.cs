namespace Time2Pay.DTO.Employments;

public interface IEmploymentModel
{
    string Name { get; }
    string? Description { get; }
    DateOnly StartDate { get; }
    DateOnly? EndDate { get; }
}
