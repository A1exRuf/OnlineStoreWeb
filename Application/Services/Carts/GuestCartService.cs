using Application.Abstractions.Carts;
using Application.Dtos.Cart;
using Application.Dtos.CartItem;
using Application.Dtos.Product;
using Application.Exceptions;
using Application.Filters;
using Application.UseCases.Carts.Commands.AddItem;
using Application.UseCases.Carts.Commands.ChangeQuantity;
using Application.UseCases.Carts.Commands.DeleteItem;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Mapster;

namespace Application.Services.Carts;

public class GuestCartService : ICartService
{
    private readonly IGuestCartStorage _guestCartStorage;
    private readonly IRepository<Product> _productRepository;

    public GuestCartService(
        IGuestCartStorage guestCartStorage,
        IRepository<Product> productRepository)
    {
        _guestCartStorage = guestCartStorage;
        _productRepository = productRepository;
    }

    public async Task<Guid> AddItemAsync(
        AddCartItemCommand request, 
        CancellationToken cancellationToken)
    {
        var cart = await _guestCartStorage.GetCartAsync()
            ?? throw new CartNotInitializedException();

        var cartItem = new GuestCartItemDto(
            cart.Id,
            request.ProductId,
            request.Quantity);

        cart.Items.Add(cartItem);

        await _guestCartStorage.SaveCartAsync(cart);

        return cart.Id;
    }

    public async Task ChangeItemQuantityAsync(
        ChangeCartItemQuantityCommand request, 
        CancellationToken cancellationToken)
    {
        var cart = await _guestCartStorage.GetCartAsync()
            ?? throw new CartNotInitializedException();

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

        await _guestCartStorage.SaveCartAsync(cart);
    }

    public async Task DeleteItemAsync(
        DeleteCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var cart = await _guestCartStorage.GetCartAsync()
            ?? throw new CartNotInitializedException();

        var updatedItems = cart.Items
            .Where(x => x.Id != request.Id)
            .ToList();

        cart = cart with { Items = updatedItems };

        await _guestCartStorage.SaveCartAsync(cart);
    }

    public async Task<CartDto> GetCartAsync(
        CancellationToken cancellationToken)
    {
        var guestCart = await _guestCartStorage.GetCartAsync()
            ?? throw new CartNotInitializedException();

        var cart = guestCart.Adapt<CartDto>();

        for (int i = 0; i < guestCart.Items.Count; i++)
        {
            var product = await _productRepository.GetAsync<ProductDto>(
                filter: new ProductFilter { Id = guestCart.Items[i].ProductId },
                cancellationToken: cancellationToken);

            if (product == null)
                throw new NotFoundByIdException<Product>(guestCart.Items[i].ProductId);

            cart.Items[i] = cart.Items[i] with { Product = product };
        }

        return cart;
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
            throw new NotEnoughProductInStockException("product");
    }
}
