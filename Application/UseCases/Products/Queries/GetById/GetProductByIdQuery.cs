using Application.Abstractions.Messaging;

namespace Application.UseCases.Products.Queries.GetById;

public record GetProductByIdQuery(Guid Id) : IQuery<GetProductsByIdResponse>;
