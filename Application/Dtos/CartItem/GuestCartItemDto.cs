namespace Application.Dtos.CartItem;

public record GuestCartItemDto(
    Guid Id,
    Guid ProductId,
    int Quantity);
