using Application.Dtos.ProductImage;

namespace Application.UseCases.Products.Queries.Get;

public record GetProductsResponse(
    Guid Id,
    string Name,
    decimal Price,
    int StockQuantity,
    List<ProductImageDto> Images);
