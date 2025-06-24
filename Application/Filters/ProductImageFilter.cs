using Domain.Abstractions;
using Domain.Entities;

namespace Application.Filters;

public class ProductImageFilter : IFilter<ProductImage>
{
    public Guid? Id { get; set; }

    IQueryable<ProductImage> IFilter<ProductImage>.ApplyFilter(IQueryable<ProductImage> query)
    {
        if (Id != null)
        {
            query = query.Where(x => x.Id == Id);
        }

        return query;
    }
}
