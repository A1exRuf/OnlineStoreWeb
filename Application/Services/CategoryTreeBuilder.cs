using Application.Abstractions;
using Application.Dtos.Category;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.Services;

public class CategoryTreeBuilder : ICategoryTreeBuilder
{
    private readonly IRepository<Category> _categoryRepository;

    public CategoryTreeBuilder(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryWithChildrenDto> BuildTreeAsync(
        CategoryWithChildrenDto root, 
        CancellationToken cancellationToken)
    {
        var children = await _categoryRepository.GetListAsync<CategoryWithChildrenDto>(
            filter: new CategoryFilter { ParentCategoryId = root.Id },
            cancellationToken: cancellationToken
        );

        var subTrees = new List<CategoryWithChildrenDto>();
        foreach (var child in children)
        {
            subTrees.Add(await BuildTreeAsync(child, cancellationToken));
        }

        return root with { SubCategories = subTrees };
    }
}
