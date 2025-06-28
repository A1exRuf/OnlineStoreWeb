using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Search;

public class ProductSearch : ISearch<Product>
{
    private readonly string? _searchTerm;

    public ProductSearch(string? searchTerm)
    {
        _searchTerm = searchTerm;
    }

    public IQueryable<Product> ApplySearch(IQueryable<Product> query)
    {
        if (!string.IsNullOrWhiteSpace(_searchTerm))
        {
            query = query.Where(x =>
                EF.Functions.ToTsVector("english", x.Name + " " + x.Description)
                .Matches(EF.Functions.PlainToTsQuery("english", _searchTerm)));
        }

        return query;
    }
}
