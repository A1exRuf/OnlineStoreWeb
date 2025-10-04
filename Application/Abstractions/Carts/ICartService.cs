using Application.Dtos.Cart;
using Application.UseCases.Carts.Commands.AddItem;
using Application.UseCases.Carts.Commands.ChangeQuantity;
using Application.UseCases.Carts.Commands.DeleteItem;

namespace Application.Abstractions.Carts;

public interface ICartService
{
    Task<Guid> AddItemAsync(
        AddCartItemCommand request,
        CancellationToken cancellationToken);

    Task ChangeItemQuantityAsync(
        ChangeCartItemQuantityCommand request,
        CancellationToken cancellationToken);

    Task DeleteItemAsync(
        DeleteCartItemCommand request,
        CancellationToken cancellationToken);

    Task<CartDto> GetCartAsync(
        CancellationToken cancellationToken);
}