using Application.Abstractions.Messaging;

namespace Application.UseCases.Categories.Commands.Delete;

public record DeleteCategoryCommand(Guid Id) : ICommand;