using Application.Abstractions.Messaging;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Orders.Queries.GetDetailed;

public class GetDetailedQueryHandler : IQueryHandler<GetDetailedQuery, GetDetailedResponse>
{
    private readonly IRepository<Order> _orderRepository;

    public GetDetailedQueryHandler(IRepository<Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<GetDetailedResponse> Handle(GetDetailedQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetAsync<GetDetailedResponse>(
            filter: new OrderFilter { Id = request.Id },
            cancellationToken: cancellationToken);

        if (order == null)
            throw new NotFoundByIdException<Order>(request.Id);

        return order;
    }
}
