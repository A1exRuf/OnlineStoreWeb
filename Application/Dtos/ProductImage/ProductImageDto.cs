namespace Application.Dtos.ProductImage;

public record ProductImageDto(
    Guid Id, string ImageUrl, string? AltText, int DisplayOrder);
