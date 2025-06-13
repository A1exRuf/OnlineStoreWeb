using Domain.Entities;

namespace Domain.Abstractions;

public interface IFilter<TEntity> where TEntity : Entity
{
    IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query);
}
