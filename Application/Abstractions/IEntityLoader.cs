using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Abstractions;

public interface IEntityLoader
{
    Task LoadAsync<TEntity>(
        TEntity entity,
        Expression<Func<TEntity, object>> navigationProperty,
        CancellationToken cancellationToken) 
        where TEntity : Entity;

    Task LoadAsync<TEntity>(
        List<TEntity> entity,
        Expression<Func<TEntity, object>> navigationProperty,
        CancellationToken cancellationToken)
        where TEntity : Entity;
}
