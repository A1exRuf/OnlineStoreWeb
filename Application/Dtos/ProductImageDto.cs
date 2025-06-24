namespace Application.Dtos;

public record ProductImageDto(
    Guid Id, string ImageUrl, string? AltText, int DisplayOrder);
