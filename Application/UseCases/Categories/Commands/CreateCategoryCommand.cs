using Application.Abstractions.Messaging;

namespace Application.UseCases.Categories.Commands;

public record CreateCategoryCommand(string Name, Guid? ParentCategoryId) : ICommand<Guid>;
