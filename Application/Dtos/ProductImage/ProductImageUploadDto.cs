using Microsoft.AspNetCore.Http;

namespace Application.Dtos.ProductImage;

public record ProductImageUploadDto(
    IFormFile File,
    string? AltText,
    int? DisplayOrder);
