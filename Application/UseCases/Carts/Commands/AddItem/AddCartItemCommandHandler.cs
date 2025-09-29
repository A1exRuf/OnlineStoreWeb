using Application.Abstractions.Carts;
using Application.Abstractions.Messaging;

namespace Application.UseCases.Carts.Commands.AddItem;

public class AddCartItemCommandHandler : ICommandHandler<AddCartItemCommand, Guid>
{
    private readonly ICartServiceFactory _cartServiceFactory;

    public AddCartItemCommandHandler(
        ICartServiceFactory cartServiceFactory)
    {
        _cartServiceFactory = cartServiceFactory;
    }

    public async Task<Guid> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        var cartService = _cartServiceFactory.Create();

        return await cartService.AddItemAsync(request, cancellationToken);
    }
}
