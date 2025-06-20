using Application.Abstractions.Messaging;

namespace Application.UseCases.Categories.Commands.Create;

public record CreateCategoryCommand(string Name, Guid? ParentCategoryId) : ICommand<Guid>;
