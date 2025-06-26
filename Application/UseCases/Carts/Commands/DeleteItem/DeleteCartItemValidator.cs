using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Carts.Commands.DeleteItem;

public class DeleteCartItemValidator : AbstractValidator<DeleteCartItemCommand>
{
    public DeleteCartItemValidator(IRepository<CartItem> cartItemRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}
