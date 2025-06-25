using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Dtos.Cart;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Mapster;

namespace Application.UseCases.Carts.Queries.Get;

public class GetCartQueryHandler : IQueryHandler<GetCartQuery, GuestCartDto>
{
    private readonly IRepository<Cart> _cartRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IGuestCartService _guestCartService;

    public GetCartQueryHandler(
        IRepository<Cart> cartRepository, 
        ICurrentUserService currentUserService, 
        IGuestCartService guestCartService)
    {
        _cartRepository = cartRepository;
        _currentUserService = currentUserService;
        _guestCartService = guestCartService;
    }

    public async Task<GuestCartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId.HasValue) // For customer
        {
            var response = await _cartRepository.GetAsync<GuestCartDto>(
                filter: new CartFilter { UserId = userId },
                cancellationToken: cancellationToken);

            return response!;
        }
        else // For guest
        {
            var cartId = _currentUserService.GuestCartId;

            var cart = await _guestCartService.GetCartAsync(cartId);

            var response = cart.Adapt<GuestCartDto>();

            return response;
        }
    }
}
