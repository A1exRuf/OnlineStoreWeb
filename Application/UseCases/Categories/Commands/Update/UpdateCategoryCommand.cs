using Application.Abstractions.Messaging;

namespace Application.UseCases.Categories.Commands.Update;

public record UpdateCategoryCommand(
    Guid Id, 
    string? Name,
    Guid? ParentCategoryId) : ICommand;
