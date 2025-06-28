using Domain.Common;

namespace Application.UseCases.Orders.Queries.GetActive;

public record GetActiveOrdersResponse(
    Guid Id,
    DateTime OrderDate,
    OrderStatus Status,
    string Address);