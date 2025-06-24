using Application.Dtos;

namespace Application.Abstractions;

public interface ICategoryTreeBuilder
{
    Task<CategoryWithChildrenDto> BuildTreeAsync(CategoryWithChildrenDto root, CancellationToken cancellationToken);
}
