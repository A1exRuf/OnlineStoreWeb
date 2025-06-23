using Application.Abstractions.Messaging;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Mapster;

namespace Application.UseCases.Products.Commands.Update;

public class UpdateProductCommandHander : ICommandHandler<UpdateProductCommand>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHander(IRepository<Product> productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetAsync(
            filter: new ProductFilter { Id = request.Id },
            asNoTracking: false,
            cancellationToken) ?? throw new 
            NotFoundByIdException<Product>(request.Id);

        request.Adapt(product);

        _productRepository.Update(product);

        await _unitOfWork.SaveChangesAsync();
    }
}
