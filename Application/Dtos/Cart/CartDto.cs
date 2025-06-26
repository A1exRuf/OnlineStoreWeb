using Application.Dtos.CartItem;

namespace Application.Dtos.Cart;

public record CartDto(
    Guid Id,
    List<CartItemDto> Items)
{
    public decimal Total => Items.Sum(x => x.Total);
}
