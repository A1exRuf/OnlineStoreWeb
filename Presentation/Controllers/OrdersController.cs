using Application.UseCases.Orders.Commands;
using Application.UseCases.Orders.Queries.GetActive;
using Application.UseCases.Orders.Queries.GetCompleted;
using Application.UseCases.Orders.Queries.GetDetailed;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class OrdersController : ApiController
{
    public OrdersController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpGet("active")]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var query = new GetActiveOrdersQuery();

        var result = await Sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("completed")]
    public async Task<IActionResult> GetСompleted(
        [FromQuery] GetCompletedOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetailed(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetDetailedQuery(id);

        var result = await Sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command);

        return Ok(result);
    }
}