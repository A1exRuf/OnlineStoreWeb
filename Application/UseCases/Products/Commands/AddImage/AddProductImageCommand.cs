using Application.Abstractions.Messaging;

namespace Application.UseCases.Products.Commands.AddImage;

public record AddProductImageCommand(
    Guid Id,
    Stream Stream,
    string ContentType,
    string? AltText,
    int? DisplayOrder) : ICommand;
