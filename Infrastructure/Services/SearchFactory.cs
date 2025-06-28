using Application.Abstractions;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Search;

namespace Infrastructure.Services;

public class SearchFactory : ISearchFactory
{
    public ISearch<TEntity>? CreateSearch<TEntity>(string? searchTerm) where TEntity : Entity
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return null;

        if (typeof(TEntity) == typeof(Product))
            return (ISearch<TEntity>)new ProductSearch(searchTerm);

        return null;
    }
}
