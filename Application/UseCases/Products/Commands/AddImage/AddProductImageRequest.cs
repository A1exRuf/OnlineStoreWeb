using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Products.Commands.AddImage;

public record AddProductImageRequest(
    IFormFile Image,
    string? AltText,
    int? DisplayOrder);
