using Application.UseCases.Products.Commands.Create;
using Application.UseCases.Products.Queries.Get;
using Application.UseCases.Products.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class ProductsController : ApiController
{
    public ProductsController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);
        var result = await Sender.Send(query);

        return Ok(result);
    }

    [Authorize(Policy = "OnlyForAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command);

        return Created(string.Empty, result);
    }
}
