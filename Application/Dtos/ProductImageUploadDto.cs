using Microsoft.AspNetCore.Http;

namespace Application.Dtos;

public record ProductImageUploadDto(
    IFormFile File,
    string? AltText,
    int? DisplayOrder);
