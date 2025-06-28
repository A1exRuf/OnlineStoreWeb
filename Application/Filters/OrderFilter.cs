using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;

namespace Application.Filters;

public class OrderFilter : IFilter<Order>
{
    public Guid? UserId { get; set; }
    public bool? OnlyActive { get; set; }
    public bool? OnlyComplited { get; set; }

    public IQueryable<Order> ApplyFilter(IQueryable<Order> query)
    {
        if (UserId != null)
        {
            query = query.Where(x => x.UserId == UserId);
        }

        if (OnlyActive == true)
        {
            query = query.Where(x => x.Status != OrderStatus.Completed);
        }

        if (OnlyComplited == true)
        {
            query = query.Where(x => x.Status == OrderStatus.Completed);
        }

        return query;
    }
}
