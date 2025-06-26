using Application.Dtos.Category;
using Application.Dtos.ProductImage;

namespace Application.UseCases.Products.Queries.GetById;

public record GetProductsByIdResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    CategoryDto Category,
    List<ProductImageDto> Images);

