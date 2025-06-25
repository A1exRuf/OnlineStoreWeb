namespace Application.Dtos.Category;

public record CategoryWithChildrenDto(
    Guid Id,
    string Name,
    List<CategoryWithChildrenDto> SubCategories);
