using Application.Abstractions.Messaging;

namespace Application.UseCases.Products.Commands.Create;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    Guid CategoryId) : ICommand<Guid>;
