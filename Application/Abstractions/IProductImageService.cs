using Application.Dtos;
using Domain.Entities;

namespace Application.Abstractions;

public interface IProductImageService
{
    public Task AddProductImage(List<ProductImageUploadDto> images, Product product, CancellationToken cancellationToken);
}
