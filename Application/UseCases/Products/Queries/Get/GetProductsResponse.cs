using Application.Dtos;

namespace Application.UseCases.Products.Queries.Get;

public record GetProductsResponse(
    Guid Id,
    string Name,
    decimal Price,
    int StockQuantity,
    List<ProductImageDto> Images);
