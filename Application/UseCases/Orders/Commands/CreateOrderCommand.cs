using Application.Abstractions.Messaging;

namespace Application.UseCases.Orders.Commands;

public record CreateOrderCommand(string Address) : ICommand<Guid>;
