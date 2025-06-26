using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Exceptions;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Carts.Commands.ChangeQuantity;

public class ChangeCartItemQuantityCommandHandler : ICommandHandler<ChangeCartItemQuantityCommand>
{
    private readonly IRepository<CartItem> _cartItemRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IGuestCartService _guestCartService;

    public ChangeCartItemQuantityCommandHandler(
        IRepository<CartItem> cartItemRepository, 
        IRepository<Product> productRepository, 
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService, 
        IGuestCartService guestCartService)
    {
        _cartItemRepository = cartItemRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _guestCartService = guestCartService;
    }

    public async Task Handle(ChangeCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId.HasValue) // For customer
        {
            var cartItem = await _cartItemRepository.GetAsync(
                filter: new CartItemFilter { Id = request.Id },
                asNoTracking: false,
                cancellationToken: cancellationToken);

            if (cartItem == null)
                throw new NotFoundByIdException<CartItem>(request.Id);

            await CheckIfEnoughProductsInStock(
                request.Quantity, 
                cartItem.ProductId, 
                cancellationToken);

            cartItem!.Quantity = request.Quantity;

            _cartItemRepository.Update(cartItem);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else // For guest
        {
            var cartId = _currentUserService.GuestCartId;

            var cart = await _guestCartService.GetCartAsync(cartId)
                ?? throw new NotFoundByIdException<Cart>(cartId);

            var cartItem = cart.Items.Where(x => x.Id == request.Id).FirstOrDefault()
                ?? throw new NotFoundByIdException<CartItem>(request.Id);

            await CheckIfEnoughProductsInStock(
                request.Quantity,
                cartItem.ProductId,
                cancellationToken);

            var updatedItems = cart.Items
                .Select(x => x.Id == request.Id ? x with { Quantity = request.Quantity } : x)
                .ToList();

            cart = cart with { Items = updatedItems };

            await _guestCartService.SaveCartAsync(cart);
        }
    }

    private async Task CheckIfEnoughProductsInStock(
        int quantity, 
        Guid productId, 
        CancellationToken cancellationToken)
    {
        bool inStock = await _productRepository.ExistsAsync(
            filter: new ProductFilter { Id = productId, StockQuantity = quantity },
            cancellationToken);

        if (!inStock)
            throw new NotEnoughProductInStockException();
    }
}
