using Application.Filters;
using Application.UseCases.Categories.Commands.Delete;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Products.Commands.Delete;

public class DeleteProductValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteProductValidator(IRepository<Product> productRepository)
    {
        RuleFor(x => x.Id).NotEmpty()
            .MustAsync(async (id, canncelationToken) =>
            {
                return await productRepository.ExistsAsync(
                    filter: new ProductFilter { Id = id },
                    canncelationToken);
            })
            .WithMessage("Product does not exist");
    }
}
