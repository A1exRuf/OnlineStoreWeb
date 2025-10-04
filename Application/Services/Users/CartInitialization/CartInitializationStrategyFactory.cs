using Application.Abstractions.Carts;
using Application.Abstractions.Users.CartInitialization;

namespace Application.Services.Users.CartInitialization;

public class CartInitializationStrategyFactory : ICartInitializationStrategyFactory
{
    private readonly IGuestCartStorage _guestCartStorage;
    private readonly NewCartInitializationStrategy _newStrategy;
    private readonly TransferGuestCartInitializationStrategy _transferStrategy;

    public CartInitializationStrategyFactory(IGuestCartStorage guestCartStorage)
    {
        _guestCartStorage = guestCartStorage;
        _newStrategy = new NewCartInitializationStrategy();
        _transferStrategy = new TransferGuestCartInitializationStrategy(guestCartStorage);
    }

    public async Task<ICartInitializationStrategy> CreateAsync()
    {
        var guestCart = await _guestCartStorage.GetCartAsync();

        return guestCart == null
            ? _newStrategy : _transferStrategy;
    }
}
