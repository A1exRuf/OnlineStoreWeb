using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Enums;
using Application.Filters;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.UseCases.Products.Queries.Get;

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PagedList<GetProductsResponse>>
{
    private readonly IRepository<Product> _productRepository;
    private readonly ISearchFactory _searchFactory;

    public GetProductsQueryHandler(
        IRepository<Product> productRepository,
        ISearchFactory searchFactory)
    {
        _productRepository = productRepository;
        _searchFactory = searchFactory;
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
            search: _searchFactory.CreateSearch<Product>(request.SearchTerm),
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
