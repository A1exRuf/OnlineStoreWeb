using FluentValidation;

namespace Application.UseCases.Orders.Commands;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required");
    }
}
