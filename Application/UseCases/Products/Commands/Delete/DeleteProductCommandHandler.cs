using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.UseCases.Products.Commands.Delete;

public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;

    public DeleteProductCommandHandler(
        IRepository<Product> productRepository, 
        IUnitOfWork unitOfWork,
        IBlobService blobService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _blobService = blobService;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetAsync(
            filter: new ProductFilter { Id = request.Id },
            cancellationToken: cancellationToken,
            includes: new[] {"Images"});

        foreach (var image in product!.Images)
        {
            await _blobService.DeleteAsync(image.Id, cancellationToken);
        }

        await _productRepository.RemoveAsync(
            filter: new ProductFilter { Id = request.Id },
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
