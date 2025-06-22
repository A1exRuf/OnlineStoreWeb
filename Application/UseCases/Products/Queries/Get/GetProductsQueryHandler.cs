using Application.Abstractions.Messaging;
using Application.Enums;
using Application.Filters;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using System.Globalization;
using System.Linq.Expressions;

namespace Application.UseCases.Products.Queries.Get;

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PagedList<GetProductsResponse>>
{
    private readonly IRepository<Product> _productRepository;

    public GetProductsQueryHandler(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedList<GetProductsResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetPagedListAsync<GetProductsResponse>(
            request.page,
            request.pageSize,
            filter: new ProductFilter
            {
                CategoryId = request.CategoryId,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                InStockOnly = request.InStock
            },
            orderBy: SelectSortExpression(request),
            descending: request.descending ?? false,
            cancellationToken: cancellationToken);

        return products;
    }

    private static Expression<Func<Product, object>> SelectSortExpression(GetProductsQuery request)
    {
        return request.SortBy switch
        {
            ProductSortBy.Price => product => product.Price,
            ProductSortBy.Popularity => product => product.PurchaseCount,
            ProductSortBy.CreatedAt => product => product.CreatedAt,
            _ => product => product.CreatedAt
        };
    }
}
