using Application.Dtos.Cart;
using Application.Dtos.CartItem;
using Application.Dtos.Product;
using Domain.Entities;
using Mapster;

namespace Application.Mapping;

public static class CartMappingConfig
{
    public static void Configure()
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.ForType<Cart, CartDto>()
        .ConstructUsing(src => new CartDto(
            src.Id,
            src.Items.Adapt<List<CartItemDto>>()));

        config.ForType<CartItem, CartItemDto>()
            .ConstructUsing(src => new CartItemDto(
                src.Id,
                src.Product.Adapt<ProductDto>(),
                src.Quantity));

        config.ForType<GuestCartDto, CartDto>()
            .ConstructUsing(src => new CartDto(
                src.Id,
                src.Items.Adapt<List<CartItemDto>>()));

        config.ForType<GuestCartItemDto, CartItemDto>()
            .ConstructUsing(src => new CartItemDto(
                src.Id,
                src.ProductId.Adapt<ProductDto>(),
                src.Quantity));
    }
}
