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

    public async Task AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public async Task<int> RemoveAsync(
        IFilter<TEntity> filter,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyFilter(filter)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public async Task<bool> ExistsAsync(
        IFilter<TEntity> filter,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyFilter(filter)
            .AnyAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(
        IFilter<TEntity> filter, 
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return await DbSet
            .ApplyIncludes(includes)
            .ApplyFilter(filter)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TDto?> GetAsync<TDto>(
        IFilter<TEntity> filter, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyFilter(filter)
            .AsNoTracking()
            .ProjectToType<TDto>()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TDto>> GetListAsync<TDto>(
        IFilter<TEntity> filter, 
        Expression<Func<TEntity, object>>? orderBy = null, 
        bool descending = false, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ApplyFilter(filter)
            .SortQuery(orderBy, descending)
            .AsNoTracking()
            .ProjectToType<TDto>()
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedList<TDto>> GetPagedListAsync<TDto>(
        int page, 
        int pageSize, 
        IFilter<TEntity> filter,
        ISearch<TEntity>? search = null,
        Expression<Func<TEntity, object>>? orderBy = null, 
        bool descending = false, 
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .ApplyFilter(filter)
            .ApplySearch(search)
            .SortQuery(orderBy, descending)
            .AsNoTracking();

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .ProjectToType<TDto>()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<TDto>(items, page, pageSize, totalCount);
    }

    private DbSet<TEntity> DbSet => _context.Set<TEntity>();
}
