using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Products.Commands.AddImage;

public class AddProductImageValidator : AbstractValidator<AddProductImageCommand>
{
    public AddProductImageValidator(IRepository<Product> productRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is requered")
            .MustAsync(async (id, cancellationToken) =>
            {
                return await productRepository.ExistsAsync(
                    filter: new ProductFilter { Id = id },
                    cancellationToken);
            })
            .WithMessage("Product does not exist");

        RuleFor(x => x.AltText)
            .MaximumLength(20);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(1).WithMessage("Display order must be a positive number")
            .When(x => x.DisplayOrder.HasValue);
    }
}
