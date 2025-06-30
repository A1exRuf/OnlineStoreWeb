using Application.Abstractions.Messaging;
using Domain.Common;

namespace Application.UseCases.Orders.Queries.GetCompleted;

public record GetCompletedOrdersQuery(
    int Page = 1,
    int PageSize = 10) : IQuery<PagedList<GetCompletedOrdersResponse>>;
