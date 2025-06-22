using Application.Dtos;

namespace Application.UseCases.Products.Queries.GetById;

public record GetProductsByIdResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    CategoryDto Category);

