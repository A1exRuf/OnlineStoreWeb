using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class CartNotInitializedException : NotFoundException
{
    public CartNotInitializedException() : base("Cart not found")
    {
    }
}
