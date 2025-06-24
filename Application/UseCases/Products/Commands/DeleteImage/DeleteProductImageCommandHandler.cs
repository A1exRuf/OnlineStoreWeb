using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Products.Commands.DeleteImage;

public class DeleteProductImageCommandHandler : ICommandHandler<DeleteProductImageCommand>
{
    private readonly IRepository<ProductImage> _productImagerepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;

    public DeleteProductImageCommandHandler(
        IRepository<ProductImage> productImagerepository, 
        IUnitOfWork unitOfWork, 
        IBlobService blobService)
    {
        _productImagerepository = productImagerepository;
        _unitOfWork = unitOfWork;
        _blobService = blobService;
    }

    public async Task Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        int deleted = await _productImagerepository.RemoveAsync(
            filter: new ProductImageFilter { Id = request.Id },
            cancellationToken);

        if (deleted != 1)
            throw new NotFoundByIdException<ProductImage>(request.Id);

        await _blobService.DeleteAsync(request.Id, cancellationToken);

        await _unitOfWork.SaveChangesAsync();
    }
}
