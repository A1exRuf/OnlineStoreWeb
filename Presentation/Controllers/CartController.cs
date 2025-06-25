using Application.UseCases.Carts.Commands.AddItem;
using Application.UseCases.Carts.Commands.ChangeQuantity;
using Application.UseCases.Carts.Commands.DeleteItem;
using Application.UseCases.Carts.Queries.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class CartController : ApiController
{
    public CartController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken cancellationToken)
    {
        var query = new GetCartQuery();

        var result = await Sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(
        [FromBody] AddCartItemCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpPut("items/{id}/quantity")]
    public async Task<IActionResult> ChangeQuantity(
        [FromRoute] Guid id,
        [FromBody] ChangeCartItemQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeCartItemQuantityCommand(id, request.Quantity);

        await Sender.Send(command, cancellationToken);

        return Ok();
    }

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> DeleteItem(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCartItemCommand(id);

        await Sender.Send(command, cancellationToken);

        return NoContent();
    }
}
