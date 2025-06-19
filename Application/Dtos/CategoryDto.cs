namespace Application.Dtos;

public record CategoryDto(
    Guid Id,
    string Name,
    List<CategoryDto> SubCategories);
