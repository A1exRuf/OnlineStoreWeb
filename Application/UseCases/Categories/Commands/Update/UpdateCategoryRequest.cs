namespace Application.UseCases.Categories.Commands.Update;

public record UpdateCategoryRequest(
    string? Name,
    Guid? ParentCategoryId);
