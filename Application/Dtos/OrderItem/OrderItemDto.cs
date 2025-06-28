using Application.Dtos.Product;

namespace Application.Dtos.OrderItem;

public record OrderItemDto(
    Guid Id,
    ProductDto Product,
    int Quantity,
    decimal UnitPrice);
