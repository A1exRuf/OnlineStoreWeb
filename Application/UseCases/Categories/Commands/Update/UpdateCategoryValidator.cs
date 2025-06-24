using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Categories.Commands.Update;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator(
        IRepository<Category> categoryRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required")
            .MustAsync(async (id, cancellationToken) =>
            {
                return await categoryRepository.ExistsAsync(
                    filter: new CategoryFilter { Id = id },
                    cancellationToken);
            })
            .WithMessage("Category does not exist");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(x => x.ParentCategoryId)
        .MustAsync(async (command, parentId, context, cancellationToken) =>
        {
            if (!parentId.HasValue)
                return true;

            if (parentId.Value == command.Id)
                return false;

            return await categoryRepository.ExistsAsync(
                new CategoryFilter { Id = parentId.Value },
                cancellationToken);
        })
        .WithMessage(x => x.ParentCategoryId == x.Id
            ? "Category cannot reference to itself"
            : "Parent category does not exist");
    }
}
