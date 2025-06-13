using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly ApplicationDbContext _context;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Methods
    // Add
    public async Task AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default)
    {
        await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    // Remove
    public async Task<int> RemoveAsync(
        IFilter<TEntity> filter,
        CancellationToken cancellationToken = default)
    {
        // Build filtered query for deletion
        var query = _context.Set<TEntity>().AsQueryable();
        query = filter.ApplyFilter(query);

        // Execute deletion and return number of affected rows
        var quantity = await query.ExecuteDeleteAsync(cancellationToken);

        return quantity;
    }

    // Update
    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }

    // Exists
    public async Task<bool> ExistsAsync(
        IFilter<TEntity> filter,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        query = ApplyFilter(filter, query);

        return await query.AnyAsync(cancellationToken);
    }

    // Get
    public async Task<TEntity?> GetAsync(
        IFilter<TEntity> filter, 
        bool asNoTracking = true, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        query = ApplyAsNoTracking(asNoTracking, query);
        query = ApplyFilter(filter, query);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TDto?> GetAsync<TDto>(
        IFilter<TEntity> filter, 
        bool asNoTracking = true, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        query = ApplyAsNoTracking(asNoTracking, query);
        query = ApplyFilter(filter, query);

        return await query.ProjectToType<TDto>().FirstOrDefaultAsync(cancellationToken);
    }

    // Get List
    public async Task<List<TDto>> GetListAsync<TDto, TKey>(
        IFilter<TEntity> filter, 
        bool asNoTracking = true, 
        Expression<Func<TEntity, TKey>>? orderBy = null, bool descending = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        query = ApplyAsNoTracking(asNoTracking, query);
        query = ApplyFilter(filter, query);
        query = SortQuery(orderBy, descending, query);

        return await query.ProjectToType<TDto>().ToListAsync(cancellationToken);
    }

    // Get Paged List
    public async Task<PagedList<TDto>> GetPagedListAsync<TDto, TKey>(
        int page, 
        int pageSize, 
        IFilter<TEntity> filter, 
        bool asNoTracking = true, 
        Expression<Func<TEntity, TKey>>? orderBy = null, 
        bool descending = false, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        query = ApplyAsNoTracking(asNoTracking, query);
        query = ApplyFilter(filter, query);
        query = SortQuery(orderBy, descending, query);

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .ProjectToType<TDto>()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<TDto>(items, page, pageSize, totalCount);
    }


    // Helper methods
    private static IQueryable<TEntity> ApplyAsNoTracking(bool asNoTracking, IQueryable<TEntity> query)
    {
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    private static IQueryable<TEntity> ApplyFilter(IFilter<TEntity> filter, IQueryable<TEntity> query)
    {
        query = filter.ApplyFilter(query);
        return query;
    }

    private IQueryable<TEntity> SortQuery<TSort>(Expression<Func<TEntity, TSort>>? orderBy, bool descending, IQueryable<TEntity> query)
    {
        if (orderBy != null)
        {
            if (descending)
                query = query.OrderByDescending(orderBy);
            else
                query = query.OrderBy(orderBy);
        }
        else
        {
            query = query.OrderBy(x => x.Id);
        }

        return query;
    }
}
