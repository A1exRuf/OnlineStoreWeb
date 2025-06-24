using Application.Abstractions.Messaging;

namespace Application.UseCases.Products.Commands.Delete;

public record DeleteProductCommand(Guid Id) : ICommand;
