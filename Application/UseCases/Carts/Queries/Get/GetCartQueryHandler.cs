using Application.Abstractions.Carts;
using Application.Abstractions.Messaging;
using Application.Dtos.Cart;

namespace Application.UseCases.Carts.Queries.Get;

public class GetCartQueryHandler : IQueryHandler<GetCartQuery, CartDto>
{
    private readonly ICartServiceFactory _cartServiceFactory;

    public GetCartQueryHandler(ICartServiceFactory cartServiceFactory)
    {
        _cartServiceFactory = cartServiceFactory;
    }

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cartService = _cartServiceFactory.Create();

        return await cartService.GetCartAsync(cancellationToken);
    }
}
