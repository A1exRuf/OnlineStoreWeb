using Application.Abstractions.Messaging;
using Domain.Common;

namespace Application.UseCases.Orders.Queries.GetCompleted;

public record GetCompletedOrdersQuery(
    int Page,
    int PageSize) : IQuery<PagedList<GetCompletedOrdersResponse>>;
