using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> ApplyIncludes<TEntity>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, object>>[] includes)
        where TEntity : Entity
    {
        if (includes == null)
            return query;

        return includes.Aggregate(query, (current, include) => current.Include(include));
    }

    public static IQueryable<TEntity> ApplyFilter<TEntity>(
        this IQueryable<TEntity> query,
        IFilter<TEntity> filter)
        where TEntity : Entity
    {
        return filter.ApplyFilter(query);
    }

    public static IQueryable<TEntity> ApplySearch<TEntity>(
        this IQueryable<TEntity> query,
        ISearch<TEntity>? search)
        where TEntity: Entity
    {
        return search == null ? query : search.ApplySearch(query);
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
