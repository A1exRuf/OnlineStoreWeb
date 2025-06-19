using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Categories.Commands;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator(IRepository<Category> repository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(x => x.ParentCategoryId)
            .MustAsync(async (parentCategoryId, cancellationToken) =>
            {
                if (parentCategoryId == null)
                    return true;

                return await repository.ExistsAsync(
                    new CategoryFilter { Id = parentCategoryId }, cancellationToken);
            }).WithMessage("Parent category does not exist");
    }
}
