using Application.Dtos.Category;

namespace Application.Abstractions;

public interface ICategoryTreeBuilder
{
    Task<CategoryWithChildrenDto> BuildTreeAsync(CategoryWithChildrenDto root, CancellationToken cancellationToken);
}
