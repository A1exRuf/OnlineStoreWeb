using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Exceptions;
using Application.Filters;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;

namespace Application.UseCases.Orders.Queries.GetCompleted;

public class GetCompletedOrdersQueryHandler : IQueryHandler<GetCompletedOrdersQuery, PagedList<GetCompletedOrdersResponse>>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCompletedOrdersQueryHandler(IRepository<Order> orderRepository, ICurrentUserService currentUserService)
    {
        _orderRepository = orderRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedList<GetCompletedOrdersResponse>> Handle(GetCompletedOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UserNotAuthenticatedException();

        var orders = await _orderRepository.GetPagedListAsync<GetCompletedOrdersResponse>(
            request.Page,
            request.PageSize,
            filter: new OrderFilter { UserId = userId, OnlyComplited = true },
            orderBy: x => x.CompletedDate!,
            cancellationToken: cancellationToken);

        return orders;
    }
}
