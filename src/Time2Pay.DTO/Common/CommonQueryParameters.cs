using Microsoft.AspNetCore.Mvc;

namespace Time2Pay.DTO.Common;

public sealed record CommonQueryParameters : IQueryParameters
{
    [FromQuery(Name = "q")] public string? Search { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}
