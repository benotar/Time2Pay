namespace Time2Pay.DTO.Common;

public interface IPaginationResult<T>
{
    List<T> Items { get; init; }

    int Page { get; init; }

    int PageSize { get; init; }

    int TotalCount { get; init; }

    int TotalPages { get; }

    bool HasPreviousPage { get; }

    bool HasNextPage { get; }
}
