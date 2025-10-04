namespace Application.Abstractions.Users.CartInitialization;

public interface ICartInitializationStrategyFactory
{
    Task<ICartInitializationStrategy> CreateAsync();
}
