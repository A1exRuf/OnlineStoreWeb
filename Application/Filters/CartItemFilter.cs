using Domain.Abstractions;
using Domain.Entities;

namespace Application.Filters;

public class CartItemFilter : IFilter<CartItem>
{
    public Guid? Id { get; set; }
    public IQueryable<CartItem> ApplyFilter(IQueryable<CartItem> query)
    {
        if (Id != null)
        {
            query = query.Where(x => x.Id == Id);
        }

        return query;
    }

    public string GetCacheKey()
    {
        return $"{nameof(Id)}={Id?.ToString() ?? "null"}";
    }
}
