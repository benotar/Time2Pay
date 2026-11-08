using Microsoft.AspNetCore.Mvc;

namespace Time2Pay.DTO.Common;

public interface IQueryParameters
{
    [FromQuery(Name = "q")] string? Search { get; init; }
    
    int Page { get; init; }

    int PageSize { get; init; }
}
