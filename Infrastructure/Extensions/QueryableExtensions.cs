using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> ApplyAsNoTracking<TEntity>(
        this IQueryable<TEntity> query,
        bool asNoTracking) 
        where TEntity : Entity
    {
        return asNoTracking ? query.AsNoTracking() : query;
    }

    public static IQueryable<TEntity> ApplyIncludes<TEntity>(
        this IQueryable<TEntity> query,
        string[]? includeStrings)
        where TEntity : Entity
    {
        if (includeStrings == null || !includeStrings.Any())
            return query;

        foreach (var include in includeStrings)
        {
            query = query.Include(include);
        }

        return query;
    }

    public static IQueryable<TEntity> ApplyFilter<TEntity>(
        this IQueryable<TEntity> query,
        IFilter<TEntity> filter)
        where TEntity : Entity
    {
        return filter.ApplyFilter(query);
    }

    public static IQueryable<TEntity> SortQuery<TEntity>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, object>>? orderBy,
        bool descending)
        where TEntity : Entity
    {
        if (orderBy != null)
        {
            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        } 
        else
        {
            query = descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
        }
        return query;
    }
}
