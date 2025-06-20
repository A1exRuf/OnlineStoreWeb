using Application.Dtos;

namespace Application.Abstractions;

public interface ICategoryTreeBuilder
{
    Task<CategoryDto> BuildTreeAsync(CategoryDto root, CancellationToken cancellationToken);
}
