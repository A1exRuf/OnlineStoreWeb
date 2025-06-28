namespace Application.UseCases.Orders.Queries.GetCompleted;

public record GetCompletedOrdersResponse(
    Guid Id,
    DateTime CompletedDate,
    string Address);