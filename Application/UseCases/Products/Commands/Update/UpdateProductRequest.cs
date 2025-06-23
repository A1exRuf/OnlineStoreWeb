namespace Application.UseCases.Products.Commands.Update;

public record UpdateProductRequest(
    string? Name,
    string? Description,
    decimal? Price,
    int? StockQuantity,
    Guid? CategoryId);
