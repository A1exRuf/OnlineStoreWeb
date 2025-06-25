using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Carts.Commands.AddItem;

public class AddCartItemValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemValidator(IRepository<Product> productRepository)
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required")
            .MustAsync(async (productId, cancellationToken) =>
            {
                return await productRepository.ExistsAsync(
                    filter: new ProductFilter { Id = productId },
                    cancellationToken);
            })
            .WithMessage("Product does not exist");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(1).WithMessage("Quntity cant be less than 1")
            .MustAsync(async (command, quantity, cancellationToken) =>
            {
                return await productRepository.ExistsAsync(
                    filter: new ProductFilter { Id = command.ProductId, StockQuantity = quantity },
                    cancellationToken);
            })
            .WithMessage("Not enough product in stock");
    }
}
