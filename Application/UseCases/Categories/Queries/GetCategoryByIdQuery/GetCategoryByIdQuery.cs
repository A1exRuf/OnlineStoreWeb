using Application.Abstractions.Messaging;
using Application.Dtos.Category;

namespace Application.UseCases.Categories.Queries.GetCategoryByIdQuery;

public record GetCategoryByIdQuery(Guid Id) : IQuery<CategoryWithChildrenDto>;
