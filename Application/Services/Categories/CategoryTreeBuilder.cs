using Application.Abstractions;
using Application.Dtos.Category;

namespace Application.Services.Categories;

public class CategoryTreeBuilder : ICategoryTreeBuilder
{
    public CategoryWithChildrenDto BuildTree(
        CategoryWithChildrenDto root,
        List<CategoryWithChildrenDto> flatList)
    {
        var lookup = flatList.ToDictionary(x => x.Id);

        foreach (var category in flatList)
        {
            if (category.ParentCategoryId != null &&
                lookup.TryGetValue(category.ParentCategoryId.Value, out var parent))
            {
                parent.SubCategories.Add(category);
            }
        }

        return root;
    }

    public List<CategoryWithChildrenDto> BuildForest(List<CategoryWithChildrenDto> flatList)
    {
        var lookup = flatList.ToDictionary(x => x.Id);
        var roots = new List<CategoryWithChildrenDto>();

        foreach (var category in flatList)
        {
            if (category.ParentCategoryId == null)
            {
                roots.Add(category);
            }
            else if (lookup.TryGetValue(category.ParentCategoryId.Value, out var parent))
            {
                parent.SubCategories.Add(category);
            }
        }

        return roots;
    }
}
