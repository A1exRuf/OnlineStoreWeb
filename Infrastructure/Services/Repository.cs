using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Extensions;
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

    // Add
    public async Task AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    // Remove
    public async Task<int> RemoveAsync(
        IFilter<TEntity> filter,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyFilter(filter)
            .ExecuteDeleteAsync(cancellationToken);
    }

    // Update
    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    // Exists
    public async Task<bool> ExistsAsync(
        IFilter<TEntity> filter,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyFilter(filter)
            .AnyAsync(cancellationToken);
    }

    // Get
    public async Task<TEntity?> GetAsync(
        IFilter<TEntity> filter, 
        bool asNoTracking = true, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyAsNoTracking(asNoTracking)
            .ApplyFilter(filter)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TDto?> GetAsync<TDto>(
        IFilter<TEntity> filter, 
        bool asNoTracking = true, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyAsNoTracking(asNoTracking)
            .ApplyFilter(filter)
            .ProjectToType<TDto>()
            .FirstOrDefaultAsync(cancellationToken);
    }

    // Get List
    public async Task<List<TDto>> GetListAsync<TDto, TKey>(
        IFilter<TEntity> filter, 
        bool asNoTracking = true, 
        Expression<Func<TEntity, TKey>>? orderBy = null, 
        bool descending = false, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyAsNoTracking(asNoTracking)
            .ApplyFilter(filter)
            .SortQuery(orderBy, descending)
            .ProjectToType<TDto>()
            .ToListAsync(cancellationToken);
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
        var query = DbSet
            .ApplyAsNoTracking(asNoTracking)
            .ApplyFilter(filter)
            .SortQuery(orderBy, descending);

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .ProjectToType<TDto>()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<TDto>(items, page, pageSize, totalCount);
    }

    // Helper method
    private DbSet<TEntity> DbSet => _context.Set<TEntity>();
}
