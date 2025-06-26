namespace Application.Dtos.Product;

public record ProductDto(
    Guid Id,
    string Name,
    decimal Price);
