using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Carts.Commands.DeleteItem;

public class DeleteCartItemCommandHandler : ICommandHandler<DeleteCartItemCommand>
{
    private readonly IRepository<CartItem> _cartItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IGuestCartService _guestCartService;

    public DeleteCartItemCommandHandler(
        IRepository<CartItem> cartItemRepository, 
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService, 
        IGuestCartService guestCartService)
    {
        _cartItemRepository = cartItemRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _guestCartService = guestCartService;
    }

    public async Task Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId.HasValue)
            await DeleteItemFromCustomerCart(request, cancellationToken);
        else
            await DeleteItemFromGuestCart(request);
    }

    private async Task DeleteItemFromGuestCart(DeleteCartItemCommand request)
    {
        var cartId = _currentUserService.GuestCartId;

        var cart = await _guestCartService.GetCartAsync(cartId)
            ?? throw new NotFoundByIdException<Cart>(cartId);

        var updatedItems = cart.Items
            .Where(x => x.Id != request.Id)
            .ToList();

        cart = cart with { Items = updatedItems };

        await _guestCartService.SaveCartAsync(cart);
    }

    private async Task DeleteItemFromCustomerCart(DeleteCartItemCommand request, CancellationToken cancellationToken)
    {
        int removed = await _cartItemRepository.RemoveAsync(
            filter: new CartItemFilter { Id = request.Id },
            cancellationToken);

        if (removed != 1)
            throw new NotFoundByIdException<CartItem>(request.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
