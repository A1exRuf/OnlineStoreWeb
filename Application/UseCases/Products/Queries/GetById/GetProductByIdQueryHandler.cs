using Application.Abstractions.Messaging;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Products.Queries.GetById;

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, GetProductsByIdResponse>
{
    private readonly IRepository<Product> _productRepository;

    public GetProductByIdQueryHandler(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<GetProductsByIdResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetAsync<GetProductsByIdResponse>(
            filter: new ProductFilter { Id = request.Id },
            cancellationToken: cancellationToken) 
        ?? throw new NotFoundByIdException<Product>(request.Id);

        return product;
    }
}
