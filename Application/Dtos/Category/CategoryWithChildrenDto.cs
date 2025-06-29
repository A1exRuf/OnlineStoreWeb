namespace Application.Dtos.Category;

public record CategoryWithChildrenDto(
    Guid Id,
    string Name,
    Guid? ParentCategoryId)
{
    public List<CategoryWithChildrenDto> SubCategories { get; init; } = [];
}
