using Application.Dtos.Category;

namespace Application.Abstractions;

public interface ICategoryTreeBuilder
{
    CategoryWithChildrenDto BuildTree(
        CategoryWithChildrenDto root, 
        List<CategoryWithChildrenDto> flatList);

    List<CategoryWithChildrenDto> BuildForest(List<CategoryWithChildrenDto> flatList);
}
