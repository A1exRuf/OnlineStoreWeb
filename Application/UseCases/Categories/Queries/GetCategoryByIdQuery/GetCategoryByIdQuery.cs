using Application.Abstractions.Messaging;
using Application.Dtos;

namespace Application.UseCases.Categories.Queries.GetCategoryByIdQuery;

public record GetCategoryByIdQuery(Guid Id) : IQuery<CategoryDto>;
