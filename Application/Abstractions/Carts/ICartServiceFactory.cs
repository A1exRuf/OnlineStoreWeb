namespace Application.Abstractions.Carts;

public interface ICartServiceFactory
{
    ICartService Create();
}
