using Application.Abstractions.Messaging;
using Application.Enums;
using Domain.Common;

namespace Application.UseCases.Products.Queries.Get;

public record GetProductsQuery(
    string? SearchTerm,
    Guid? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? InStock,
    ProductSortBy? SortBy,
    bool? descending,
    int page = 1,
    int pageSize = 10) : IQuery<PagedList<GetProductsResponse>>;
