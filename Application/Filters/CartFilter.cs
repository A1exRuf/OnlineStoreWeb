using Domain.Abstractions;
using Domain.Entities;

namespace Application.Filters;

public class CartFilter : IFilter<Cart>
{
    public Guid? UserId { get; set; }

    public IQueryable<Cart> ApplyFilter(IQueryable<Cart> query)
    {
        if (UserId != null)
        {
            query = query.Where(x => x.UserId == UserId);
        }

        return query;
    }

    public string GetCacheKey()
    {
        return $"{nameof(UserId)}={UserId?.ToString() ?? "null"}";
    }
}
