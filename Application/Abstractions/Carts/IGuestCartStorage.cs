using Application.Dtos.Cart;

namespace Application.Abstractions.Carts;

public interface IGuestCartStorage
{
    Task<GuestCartDto?> GetCartAsync();
    Task SaveCartAsync(GuestCartDto cart);
    Task DeleteCartAsync();
    Task TouchAsync();
}
