using Application.Abstractions.Carts;
using Application.Abstractions.Users.CartInitialization;
using Application.Exceptions;
using Domain.Entities;
using Mapster;

namespace Application.Services.Users.CartInitialization;

public class TransferGuestCartInitializationStrategy : ICartInitializationStrategy
{
    private readonly IGuestCartStorage _guestCartStorage;

    public TransferGuestCartInitializationStrategy(
        IGuestCartStorage guestCartStorage)
    {
        _guestCartStorage = guestCartStorage;
    }

    public async Task InitializeCartAsync(
        User user, 
        CancellationToken cancellation)
    {
        var guestCart = await _guestCartStorage.GetCartAsync()
            ?? throw new GuestCartNotInitializedException();

        Cart cart = new Cart();
        cart.Items.AddRange(guestCart.Items.Adapt<List<CartItem>>());

        user.Cart = cart;

        await _guestCartStorage.DeleteCartAsync();
    }
}
