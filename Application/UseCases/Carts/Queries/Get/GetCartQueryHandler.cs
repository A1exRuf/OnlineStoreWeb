using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Dtos.Cart;
using Application.Dtos.Product;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Mapster;

namespace Application.UseCases.Carts.Queries.Get;

public class GetCartQueryHandler : IQueryHandler<GetCartQuery, CartDto>
{
    private readonly IRepository<Cart> _cartRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IGuestCartService _guestCartService;

    public GetCartQueryHandler(
        IRepository<Cart> cartRepository, 
        IRepository<Product> productRepository, 
        ICurrentUserService currentUserService, 
        IGuestCartService guestCartService)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
        _guestCartService = guestCartService;
    }

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId.HasValue) // For customer
        {
            var cart = await _cartRepository.GetAsync<CartDto>(
                filter: new CartFilter { UserId = userId },
                cancellationToken: cancellationToken);

            return cart;
        }
        else // For guest
        {
            var cartId = _currentUserService.GuestCartId;

            var guestCart = await _guestCartService.GetCartAsync(cartId)
                ?? throw new NotFoundByIdException<Cart>(cartId);

            var cart = guestCart.Adapt<CartDto>();

            for (int i = 0; i < guestCart.Items.Count; i++)
            {
                var product = await _productRepository.GetAsync<ProductDto>(
                    filter: new ProductFilter { Id = guestCart.Items[i].ProductId },
                    cancellationToken: cancellationToken);

                if(product == null)
                    throw new NotFoundByIdException<Product>(guestCart.Items[i].ProductId);

                cart.Items[i] = cart.Items[i] with { Product = product };
            }


            return cart;
        }
    }
}
