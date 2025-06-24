using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Dtos;
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
        var roots = await _categoryRepository.GetListAsync<CategoryWithChildrenDto>(
            filter: new CategoryFilter { OnlyRoots = true },
            orderBy: x => x.Name,
            cancellationToken: cancellationToken);

        var result = new List<CategoryWithChildrenDto>();

        foreach (var root in roots)
        {
            var tree = await _categoryTreeBuilder.BuildTreeAsync(root, cancellationToken);
            result.Add(tree);
        }

        return result;
    }
}