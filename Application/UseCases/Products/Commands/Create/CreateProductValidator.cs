using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Products.Commands.Create;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator(
        IRepository<Product> productRepository,
        IRepository<Category> categoryRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is requered")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
            .MustAsync(async (name, cancelationToken) =>
            {
                return !await productRepository.ExistsAsync(
                    filter: new ProductFilter { Name = name },
                    cancelationToken);
            }).WithMessage("Product with such name already exists");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 4000 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero")
            .Must(price =>
            {
                return decimal.Round(price, 2) == price;
            }).WithMessage("Invalid price format");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

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
            }).WithMessage("Category should not have subcategories");
    }
}
