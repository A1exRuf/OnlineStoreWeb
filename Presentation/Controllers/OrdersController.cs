using Application.UseCases.Orders.Commands;
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
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command);

        return Ok(result);
    }
}