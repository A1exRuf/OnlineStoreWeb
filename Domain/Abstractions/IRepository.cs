using Domain.Common;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Abstractions;

public interface IRepository<TEntity> where TEntity : Entity
{
    // Add
    Task AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default);

    // Remove
    Task<int> RemoveAsync(
        IFilter<TEntity> filter,
        CancellationToken cancellationToken = default);

    // Update
    void Update(TEntity entity);

    // Exists
    Task<bool> ExistsAsync(
        IFilter<TEntity> filter,
        CancellationToken cancellationToken = default);

    // Get
    Task<TEntity?> GetAsync(
        IFilter<TEntity> filter,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);
    Task<TDto?> GetAsync<TDto>(
        IFilter<TEntity> filter,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);

    // Get List
    Task<List<TDto>> GetListAsync<TDto, TKey>(
        IFilter<TEntity> filter,
        bool asNoTracking = true,
        Expression<Func<TEntity, TKey>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default);

    // Get Paged List
    Task<PagedList<TDto>> GetPagedListAsync<TDto, TKey>(
        int page,
        int pageSize,
        IFilter<TEntity> filter,
        bool asNoTracking = true,
        Expression<Func<TEntity, TKey>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default);
}
