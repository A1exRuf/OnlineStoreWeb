using Application.Abstractions.Messaging;
using Application.Dtos.Category;

namespace Application.UseCases.Categories.Queries.GetCategoriesQuery;

public record GetCategoriesQuery : IQuery<List<CategoryWithChildrenDto>>;
