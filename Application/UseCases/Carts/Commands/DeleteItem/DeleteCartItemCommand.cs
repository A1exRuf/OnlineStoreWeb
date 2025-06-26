using Application.Abstractions.Messaging;

namespace Application.UseCases.Carts.Commands.DeleteItem;

public record DeleteCartItemCommand(Guid Id) : ICommand;
