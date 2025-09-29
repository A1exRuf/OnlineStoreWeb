using Application.Abstractions;
using Application.Abstractions.Carts;

namespace Application.Services.Carts;

public class CartServiceFactory : ICartServiceFactory
{
    private readonly CustomerCartService _customerCartService;
    private readonly GuestCartService _guestCartService;
    private readonly ICurrentUserService _currentUserService;

    public CartServiceFactory(
        CustomerCartService customerCartService, 
        GuestCartService guestCartService, 
        ICurrentUserService currentUserService)
    {
        _customerCartService = customerCartService;
        _guestCartService = guestCartService;
        _currentUserService = currentUserService;
    }

    public ICartService Create()
    {
        return _currentUserService.UserId.HasValue
            ? _customerCartService
            : _guestCartService;
    }
}
