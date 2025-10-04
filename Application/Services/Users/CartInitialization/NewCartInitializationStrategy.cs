using Application.Abstractions.Users.CartInitialization;
using Domain.Entities;

namespace Application.Services.Users.CartInitialization;

public class NewCartInitializationStrategy : ICartInitializationStrategy
{
    public Task InitializeCartAsync(
        User user, 
        CancellationToken cancellation)
    {
        user.Cart = new Cart();

        return Task.CompletedTask;
    }
}
