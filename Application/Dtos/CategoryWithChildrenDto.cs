namespace Application.Dtos;

public record CategoryWithChildrenDto(
    Guid Id,
    string Name,
    List<CategoryWithChildrenDto> SubCategories);
