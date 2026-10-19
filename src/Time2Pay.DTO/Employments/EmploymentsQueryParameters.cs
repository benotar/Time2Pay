using Microsoft.AspNetCore.Mvc;

namespace Time2Pay.DTO.Employments;

public sealed record EmploymentsQueryParameters
{
    [FromQuery(Name = "q")] public string? Search { get; init; }
}
