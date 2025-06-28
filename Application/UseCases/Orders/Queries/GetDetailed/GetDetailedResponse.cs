using Application.Dtos.OrderItem;
using Domain.Common;

namespace Application.UseCases.Orders.Queries.GetDetailed;

public record GetDetailedResponse(
    Guid Id,
    OrderStatus Status,
    DateTime OrderDate,
    DateTime? PaidDate,
    DateTime? ShippingDate,
    DateTime? CompletedDate,
    string Address,
    List<OrderItemDto> Items)
{
    public decimal Total => Items.Sum(x => x.Total);
}
