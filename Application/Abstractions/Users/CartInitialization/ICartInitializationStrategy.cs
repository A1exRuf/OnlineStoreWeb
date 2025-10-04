using Domain.Entities;

namespace Application.Abstractions.Users.CartInitialization;

public interface ICartInitializationStrategy
{
    Task InitializeCartAsync(
        User user,
        CancellationToken cancellation);
}
