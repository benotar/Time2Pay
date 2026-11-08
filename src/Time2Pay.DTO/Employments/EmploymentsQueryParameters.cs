using Microsoft.AspNetCore.Mvc;
using Time2Pay.DTO.Common;

namespace Time2Pay.DTO.Employments;

public sealed record EmploymentsQueryParameters : IQueryParameters
{
    [FromQuery(Name = "q")] public string? Search { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}
