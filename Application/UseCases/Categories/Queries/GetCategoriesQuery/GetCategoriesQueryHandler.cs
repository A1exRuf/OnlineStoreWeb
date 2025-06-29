using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Dtos.Category;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.UseCases.Categories.Queries.GetCategoriesQuery;

public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, List<CategoryWithChildrenDto>>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly ICategoryTreeBuilder _categoryTreeBuilder;

    public GetCategoriesQueryHandler(IRepository<Category> categoryRepository, ICategoryTreeBuilder categoryTreeBuilder)
    {
        _categoryRepository = categoryRepository;
        _categoryTreeBuilder = categoryTreeBuilder;
    }

    public async Task<List<CategoryWithChildrenDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetListAsync<CategoryWithChildrenDto>(
            filter: new CategoryFilter {},
            orderBy: x => x.Name,
            cancellationToken: cancellationToken);

        var response = _categoryTreeBuilder.BuildForest(categories);

        return response;
    }
}