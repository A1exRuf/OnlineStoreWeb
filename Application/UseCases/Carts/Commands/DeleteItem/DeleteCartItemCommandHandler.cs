using Application.Abstractions.Carts;
using Application.Abstractions.Messaging;

namespace Application.UseCases.Carts.Commands.DeleteItem;

public class DeleteCartItemCommandHandler : ICommandHandler<DeleteCartItemCommand>
{
    private readonly ICartServiceFactory _cartServiceFactory;

    public DeleteCartItemCommandHandler(ICartServiceFactory cartServiceFactory)
    {
        _cartServiceFactory = cartServiceFactory;
    }

    public async Task Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
    {
        var cartService = _cartServiceFactory.Create();

        await cartService.DeleteItemAsync(request, cancellationToken);
    }
}
