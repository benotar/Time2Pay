using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;

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
}
