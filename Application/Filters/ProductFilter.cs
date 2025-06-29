using Domain.Abstractions;
using Domain.Entities;

namespace Application.Filters;

public class ProductFilter : IFilter<Product>
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? InStockOnly { get; set; }

    public int? StockQuantity {  get; set; }

    public IQueryable<Product> ApplyFilter(IQueryable<Product> query)
    {
        if (Id != null)
        {
            query = query.Where(x => x.Id == Id);
        }

        if (!string.IsNullOrWhiteSpace(Name))
        {
            query = query.Where(x => x.Name == Name);
        }

        if (CategoryId != null)
        {
            query = query.Where(x => x.CategoryId == CategoryId);
        }

        if (MinPrice != null)
        {
            query = query.Where(x => x.Price > MinPrice);
        }

        if (MaxPrice != null)
        {
            query = query.Where(x => x.Price < MaxPrice);
        }

        if (InStockOnly != null)
        {
            query = query.Where(x => x.StockQuantity > 0);
        }

        if (StockQuantity != null)
        {
            query = query.Where(x => x.StockQuantity >= StockQuantity);
        }

        return query;
    }

    public string GetCacheKey()
    {
        return $"{nameof(Id)}={Id?.ToString() ?? "null"}:" +
            $"{nameof(Name)}={Name ?? "null"}:" +
            $"{nameof(CategoryId)}={CategoryId?.ToString() ?? "null"}:" +
            $"{nameof(MinPrice)}={MinPrice?.ToString() ?? "null"}:" +
            $"{nameof(MaxPrice)}={MaxPrice?.ToString() ?? "null"}:" +
            $"{nameof(InStockOnly)}={InStockOnly?.ToString() ?? "null"}:" +
            $"{nameof(StockQuantity)}={StockQuantity?.ToString() ?? "null"}";
    }
}
