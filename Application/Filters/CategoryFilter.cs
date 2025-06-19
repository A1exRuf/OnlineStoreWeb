using Domain.Abstractions;
using Domain.Entities;

namespace Application.Filters;

public class CategoryFilter : IFilter<Category>
{
    public Guid? Id { get; set; }

    public IQueryable<Category> ApplyFilter(IQueryable<Category> query)
    {
        if (Id != null)
        {
            query = query.Where(c => c.Id == Id);
        }

        return query;
    }
}
