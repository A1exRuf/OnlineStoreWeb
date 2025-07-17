using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.UseCases.Products.Commands.AddImage;

public class AddProductImageCommandHandler : ICommandHandler<AddProductImageCommand>
{
    private readonly IRepository<ProductImage> _productImageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobService _blobService;

    public AddProductImageCommandHandler(
        IRepository<ProductImage> productImageRepository, 
        IUnitOfWork unitOfWork,
        IBlobService blobService)
    {
        _productImageRepository = productImageRepository;
        _unitOfWork = unitOfWork;
        _blobService = blobService;
    }

    public async Task Handle(AddProductImageCommand request, CancellationToken cancellationToken)
    {
        Guid id = await _blobService.UploadAsync(request.Stream, request.ContentType, cancellationToken);
        Uri uri = await _blobService.GetUriAsync(id);

        await _productImageRepository.AddAsync(new(
            id, 
            request.Id, 
            uri.ToString(), 
            request.AltText, 
            request.DisplayOrder));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
