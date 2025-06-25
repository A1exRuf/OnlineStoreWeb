using Application.Dtos.CartItem;

namespace Application.Dtos.Cart;

public record GuestCartDto(
    Guid Id,
    List<GuestCartItemDto> Items);
