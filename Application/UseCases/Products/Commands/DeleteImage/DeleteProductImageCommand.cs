using Application.Abstractions.Messaging;

namespace Application.UseCases.Products.Commands.DeleteImage;

public record DeleteProductImageCommand(Guid Id) : ICommand;
