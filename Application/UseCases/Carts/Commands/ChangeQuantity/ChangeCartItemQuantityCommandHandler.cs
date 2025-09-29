using Application.Abstractions.Carts;
using Application.Abstractions.Messaging;

namespace Application.UseCases.Carts.Commands.ChangeQuantity;

public class ChangeCartItemQuantityCommandHandler : ICommandHandler<ChangeCartItemQuantityCommand>
{
    private readonly ICartServiceFactory _cartServiceFactory;

    public ChangeCartItemQuantityCommandHandler(ICartServiceFactory cartServiceFactory)
    {
        _cartServiceFactory = cartServiceFactory;
    }

    public async Task Handle(ChangeCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var cartService = _cartServiceFactory.Create();

        await cartService.ChangeItemQuantityAsync(request, cancellationToken);
    }
}