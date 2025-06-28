using Application.Abstractions.Messaging;

namespace Application.UseCases.Orders.Queries.GetDetailed;

public record GetDetailedQuery(Guid Id) : IQuery<GetDetailedResponse>;