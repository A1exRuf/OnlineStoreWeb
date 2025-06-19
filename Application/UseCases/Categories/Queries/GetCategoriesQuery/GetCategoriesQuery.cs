using Application.Abstractions.Messaging;
using Application.Dtos;

namespace Application.UseCases.Categories.Queries.GetCategoriesQuery;

public record GetCategoriesQuery : IQuery<List<CategoryDto>>;
