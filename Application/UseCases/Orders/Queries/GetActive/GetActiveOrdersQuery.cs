using Application.Abstractions.Messaging;

namespace Application.UseCases.Orders.Queries.GetActive;

public record GetActiveOrdersQuery : IQuery<List<GetActiveOrdersResponse>>;