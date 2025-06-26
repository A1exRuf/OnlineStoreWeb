using Application.Abstractions.Messaging;

namespace Application.UseCases.Carts.Commands.AddItem;

public record AddCartItemCommand(
    Guid ProductId,
    int Quantity) : ICommand<Guid>;
