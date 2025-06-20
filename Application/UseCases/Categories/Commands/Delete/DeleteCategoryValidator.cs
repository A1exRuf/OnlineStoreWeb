using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Categories.Commands.Delete;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator(IRepository<Category> categoryRepository)
    {
        RuleFor(c => c.Id)
            .MustAsync(async (id, cancellationToken) =>
            {
                var exists = await categoryRepository.ExistsAsync(
                    new Filters.CategoryFilter { Id = id },
                    cancellationToken);
                return exists;
            })
            .WithMessage($"Category was not found");

        RuleFor(c => c.Id)
            .MustAsync(async (id, cancellationToken) =>
            {
                var hasChildren = await categoryRepository.ExistsAsync(
                    new Filters.CategoryFilter { ParentCategoryId = id },
                    cancellationToken);
                return !hasChildren;
            })
            .WithMessage("Cannot delete a category that has subcategories");
    }
}
