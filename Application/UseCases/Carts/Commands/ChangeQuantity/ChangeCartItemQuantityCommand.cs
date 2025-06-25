using Application.Abstractions.Messaging;

namespace Application.UseCases.Carts.Commands.ChangeQuantity;

public record ChangeCartItemQuantityCommand(
    Guid Id,
    int Quantity) : ICommand;
