using Application.Abstractions;
using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class EntityLoader : IEntityLoader
{
    private readonly ApplicationDbContext _context;

    public EntityLoader(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LoadAsync<TEntity>(
        TEntity entity, 
        Expression<Func<TEntity, object>> navigationProperty, 
        CancellationToken cancellationToken) 
        where TEntity : Entity
    {
        await _context.Entry(entity)
            .Reference(navigationProperty!)
            .LoadAsync(cancellationToken);
    }

    public async Task LoadAsync<TEntity>(
        List<TEntity> entities, 
        Expression<Func<TEntity, object>> navigationProperty, 
        CancellationToken cancellationToken) 
        where TEntity : Entity
    {
        foreach (var entity in entities) 
        {
            await LoadAsync(
                entity, 
                navigationProperty, 
                cancellationToken);
        }
    }
}
