using Application.Abstractions.Messaging;

namespace Application.UseCases.Products.Commands.Update;

public record UpdateProductCommand(
    Guid Id,
    string? Name,
    string? Description,
    decimal? Price,
    int? StockQuantity,
    Guid? CategoryId) : ICommand;
