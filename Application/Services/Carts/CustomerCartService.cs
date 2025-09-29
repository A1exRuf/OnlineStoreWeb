using Application.Abstractions;
using Application.Abstractions.Carts;
using Application.Dtos;
using Application.Dtos.Cart;
using Application.Exceptions;
using Application.Filters;
using Application.UseCases.Carts.Commands.AddItem;
using Application.UseCases.Carts.Commands.ChangeQuantity;
using Application.UseCases.Carts.Commands.DeleteItem;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services.Carts;

public class CustomerCartService : ICartService
{
    private readonly IRepository<Cart> _cartRepository;
    private readonly IRepository<CartItem> _cartItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CustomerCartService(
        IRepository<Cart> cartRepository, 
        IRepository<CartItem> cartItemRepository, 
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> AddItemAsync(
        AddCartItemCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        Guid cartId = await _cartRepository.GetAsync<EntityIdDto>(
            filter: new CartFilter { UserId = userId },
            cancellationToken)
        ?? throw new CartNotInitializedException();

        var cartItem = new CartItem
        {
            CartId = cartId,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        await _cartItemRepository.AddAsync(cartItem, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return cartItem.Id;
    }

    public async Task ChangeItemQuantityAsync(
        ChangeCartItemQuantityCommand request, 
        CancellationToken cancellationToken)
    {
        var cartItem = await _cartItemRepository.GetAsync(
            filter: new CartItemFilter { Id = request.Id },
            cancellationToken)
        ?? throw new NotFoundByIdException<CartItem>(request.Id);

        cartItem.Quantity = request.Quantity;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteItemAsync(
        DeleteCartItemCommand request, 
        CancellationToken cancellationToken)
    {
        int removed = await _cartItemRepository.RemoveAsync(
            filter: new CartItemFilter { Id = request.Id },
            cancellationToken);

        if (removed != 1)
            throw new NotFoundByIdException<CartItem>(request.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<CartDto> GetCartAsync(
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UserNotAuthenticatedException();

        return await _cartRepository.GetAsync<CartDto>(
            filter: new CartFilter { UserId = userId },
            cancellationToken: cancellationToken)
        ?? throw new NotFoundByIdException<Cart>(userId);
    }
}
