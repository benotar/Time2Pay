using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Time2Pay.DTO.Common;
using Time2Pay.Shared;

namespace Time2Pay.BLL;

public static class Extensions
{
    private static readonly MethodInfo ToLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;

    private static readonly MethodInfo ContainsMethod =
        typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;

    public static IQueryable<T> WhereAnyContains<T>(this IQueryable<T> query, string? search,
        params Expression<Func<T, string?>>[] selectors)
    {
        if (string.IsNullOrWhiteSpace(search) || selectors.Length == 0)
        {
            return query;
        }

        var term = search.Trim().ToLowerInvariant();

        Expression<Func<string>> termAccessor = () => term;
        var termExpression = termAccessor.Body;

        var parameter = Expression.Parameter(typeof(T), "entity");
        Expression? predicate = null;

        foreach (var selector in selectors)
        {
            var member = ReplacingExpressionVisitor.Replace(selector.Parameters[0], parameter, selector.Body);

            var contains = Expression.Call(Expression.Call(member, ToLowerMethod),
                ContainsMethod,
                termExpression);

            predicate = predicate is null
                ? contains
                : Expression.OrElse(predicate, contains);
        }

        return query.Where(Expression.Lambda<Func<T, bool>>(predicate!, parameter));
    }

    public static async Task<PaginationResult<T>> CreatePaginationResultAsync<T>(this IQueryable<T> query, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        pageSize = Math.Clamp(pageSize, Constants.MinPageSize, Constants.MaxPageSize);

        page = Math.Clamp(page, Constants.MinPageSize, int.MaxValue / pageSize);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginationResult<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
