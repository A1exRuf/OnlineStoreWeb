using Application.Dtos.Cart;

namespace Application.Abstractions;

public interface IGuestCartService
{
    Task<GuestCartDto?> GetCartAsync(Guid id);
    Task SaveCartAsync(GuestCartDto cart);
    Task DeleteCartAsync(Guid id);
    Task TouchAsync(Guid id);
}
