using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Products.Commands.Update;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator(
        IRepository<Product> productRepository,
        IRepository<Category> categoryRepository)
    {
        RuleFor(x => x.Id).NotEmpty()
            .WithMessage("Id is requered")
            .MustAsync(async (id, cancellationToken) =>
            {
                return await productRepository.ExistsAsync(
                    filter: new ProductFilter { Id = id },
                    cancellationToken);
            })
            .WithMessage("Product is not exist");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
            .MustAsync(async (name, cancelationToken) =>
            {
                return !await productRepository.ExistsAsync(
                    filter: new ProductFilter { Name = name },
                    cancelationToken);
            }).WithMessage("Product with such name already exists")
            .When(x => x.Name != null);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 4000 characters")
            .When(x => x.Description != null);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero")
            .Must(price =>
            {
                return price == null || decimal.Round(price.Value, 2) == price;
            }).WithMessage("Invalid price format")
            .When(x => x.Price != null);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative")
            .When(x => x.StockQuantity != null);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required")
            .MustAsync(async (categoryId, cancellationToken) =>
            {
                return await categoryRepository.ExistsAsync(
                    filter: new CategoryFilter { Id = categoryId },
                    cancellationToken);
            }).WithMessage("Category does not exist")
            .MustAsync(async (categoryId, cancellationToken) =>
            {
                return !await categoryRepository.ExistsAsync(
                    filter: new CategoryFilter { ParentCategoryId = categoryId },
                    cancellationToken);
            }).WithMessage("Category should not have subcategories")
            .When(x => x.CategoryId != null);
    }
}
