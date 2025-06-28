using Application.Dtos.Product;
using Mapster;

namespace Application.Dtos.OrderItem;

public record OrderItemDto(
    Guid Id,
    ProductDto Product,
    int Quantity,
    decimal UnitPrice)
{
    public decimal Total => UnitPrice * Quantity;
}
