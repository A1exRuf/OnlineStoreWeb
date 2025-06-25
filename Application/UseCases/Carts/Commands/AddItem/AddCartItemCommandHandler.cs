using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Dtos.CartItem;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Carts.Commands.AddItem;

public class AddCartItemCommandHandler : ICommandHandler<AddCartItemCommand, Guid>
{
    private readonly IRepository<Cart> _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IGuestCartService _guestCartService;

    public AddCartItemCommandHandler(
        IRepository<Cart> cartRepository, 
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService, 
        IGuestCartService guestCartService)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _guestCartService = guestCartService;
    }

    public async Task<Guid> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId.HasValue) // For customers
        {
            var cart = await _cartRepository.GetAsync(
                filter: new CartFilter { UserId = userId },
                asNoTracking: false,
                cancellationToken,
                includes: c => c.Items);

            var cartItem = new CartItem(
                cart!.Id,
                request.ProductId,
                request.Quantity);

            cart.Items.Add(cartItem);

            await _unitOfWork.SaveChangesAsync();

            return cart.Id;
        }
        else // For guests
        {
            var cartId = _currentUserService.GuestCartId;

            var cart = await _guestCartService.GetCartAsync(cartId)
                ?? throw new NotFoundByIdException<Cart>(cartId);

            var cartItem = new CartItemDto(
                cartId,
                request.ProductId,
                request.Quantity);

            cart.Items.Add(cartItem);

            await _guestCartService.SaveCartAsync(cart);

            return cart.Id;
        }
    }
}
