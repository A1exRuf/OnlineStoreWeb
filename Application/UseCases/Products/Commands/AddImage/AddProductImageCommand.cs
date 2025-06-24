using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Products.Commands.AddImage;

public record AddProductImageCommand(
    Guid Id, 
    IFormFile Image,
    string? AltText,
    int? DisplayOrder) : ICommand;
