using Domain.Abstractions;
using Domain.Entities;

namespace Application.Abstractions;

public interface ISearchFactory
{
    ISearch<TEntity>? CreateSearch<TEntity>(string? searchTerm) where TEntity : Entity;
}