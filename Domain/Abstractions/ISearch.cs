using Domain.Entities;

namespace Domain.Abstractions;

public interface ISearch<TEntity> where TEntity : Entity
{
    IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query);
}
