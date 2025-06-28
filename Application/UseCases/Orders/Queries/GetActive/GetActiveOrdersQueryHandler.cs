using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Exceptions;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.UseCases.Orders.Queries.GetActive;

public class GetActiveOrdersQueryHandler : IQueryHandler<GetActiveOrdersQuery, List<GetActiveOrdersResponse>>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetActiveOrdersQueryHandler(
        IRepository<Order> orderRepository, 
        ICurrentUserService currentUserService)
    {
        _orderRepository = orderRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<GetActiveOrdersResponse>> Handle(GetActiveOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UserNotAuthenticatedException();

        var orders = await _orderRepository.GetListAsync<GetActiveOrdersResponse>(
            filter: new OrderFilter { UserId = userId, OnlyActive = true },
            orderBy: x => x.OrderDate,
            cancellationToken: cancellationToken);

        return orders;
    }
}
