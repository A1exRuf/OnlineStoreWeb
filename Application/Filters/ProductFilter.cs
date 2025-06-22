using Domain.Abstractions;
using Domain.Entities;

namespace Application.Filters;

public class ProductFilter : IFilter<Product>
{
    public string? Name { get; set; }

    public IQueryable<Product> ApplyFilter(IQueryable<Product> query)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            query = query.Where(x => x.Name == Name);
        }

        return query;
    }
}
