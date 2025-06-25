using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Carts.Commands.ChangeQuantity;

public class ChangeQuantityValidator : AbstractValidator<ChangeCartItemQuantityCommand>
{
    public ChangeQuantityValidator(
        IRepository<CartItem> cartItemRepository,
        IRepository<Product> productRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(1).WithMessage("Quntity cant be less than 1");
    }
}
