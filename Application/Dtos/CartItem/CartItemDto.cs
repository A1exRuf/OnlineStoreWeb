namespace Application.Dtos.CartItem;

public record CartItemDto(
    Guid Id,
    Guid ProductId,
    int Quantity);
