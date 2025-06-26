using Application.Dtos.Product;

namespace Application.Dtos.CartItem;

public record CartItemDto(
    Guid Id,
    ProductDto Product,
    int Quantity)
{
    public decimal Total => Product.Price * Quantity;
}
