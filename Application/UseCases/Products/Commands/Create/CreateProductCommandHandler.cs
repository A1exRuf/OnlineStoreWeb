using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Entities;
using Mapster;

namespace Application.UseCases.Products.Commands.Create;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IRepository<Product> productRepository, 
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = request.Adapt<Product>();

        await _productRepository.AddAsync(product);

        await _unitOfWork.SaveChangesAsync();

        return product.Id;
    }
}
