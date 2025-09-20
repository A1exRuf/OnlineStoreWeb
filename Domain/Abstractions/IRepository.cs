using Domain.Common;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Abstractions;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default);

    Task<int> RemoveAsync(
        IFilter<TEntity> filter,
        CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    Task<bool> ExistsAsync(
        IFilter<TEntity> filter,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetAsync(
        IFilter<TEntity> filter,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TDto?> GetAsync<TDto>(
        IFilter<TEntity> filter,
        CancellationToken cancellationToken = default);

    Task<List<TDto>> GetListAsync<TDto>(
        IFilter<TEntity> filter,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default);

    Task<PagedList<TDto>> GetPagedListAsync<TDto>(
        int page,
        int pageSize,
        IFilter<TEntity> filter,
        ISearch<TEntity>? search = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default);
}
