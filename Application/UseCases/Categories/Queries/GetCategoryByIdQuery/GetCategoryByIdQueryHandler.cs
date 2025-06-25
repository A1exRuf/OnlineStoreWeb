using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Dtos.Category;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Categories.Queries.GetCategoryByIdQuery;

public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, CategoryWithChildrenDto>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly ICategoryTreeBuilder _categoryTreeBuilder;

    public GetCategoryByIdQueryHandler(
        IRepository<Category> categoryRepository,
        ICategoryTreeBuilder categoryTreeBuilder)
    {
        _categoryRepository = categoryRepository;
        _categoryTreeBuilder = categoryTreeBuilder;
    }

    public async Task<CategoryWithChildrenDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var root = await _categoryRepository.GetAsync<CategoryWithChildrenDto>(
            filter: new CategoryFilter { Id = request.Id },
            cancellationToken: cancellationToken);

        if (root == null)
            throw new NotFoundByIdException<Category>(request.Id);

        var result = await _categoryTreeBuilder.BuildTreeAsync(root, cancellationToken);

        return result;
    }
}
