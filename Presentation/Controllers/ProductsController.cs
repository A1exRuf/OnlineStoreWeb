using Application.Dtos.ProductImage;
using Application.UseCases.Products.Commands.AddImage;
using Application.UseCases.Products.Commands.Create;
using Application.UseCases.Products.Commands.Delete;
using Application.UseCases.Products.Commands.DeleteImage;
using Application.UseCases.Products.Commands.Update;
using Application.UseCases.Products.Queries.Get;
using Application.UseCases.Products.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        var result = await Sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);
        var result = await Sender.Send(query, cancellationToken);

        return Ok(result);
    }

    [Authorize(Policy = "OnlyForAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);

        return Created(string.Empty, result);
    }

    [Authorize(Policy = "OnlyForAdmin")]
    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImage(
        [FromRoute] Guid id,
        IFormFile file,
        string? altText,
        int? displayOrder,
        CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();

        var command = new AddProductImageCommand(
            id,
            stream,
            file.ContentType,
            altText,
            displayOrder);

        await Sender.Send(command, cancellationToken);

        return Ok();
    }

    [Authorize(Policy = "OnlyForAdmin")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(
    [FromRoute] Guid id,
    [FromBody] UpdateProductRequest request,
    CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity,
            request.CategoryId);

        await Sender.Send(command, cancellationToken);

        return Ok();
    }

    [Authorize(Policy = "OnlyForAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        [FromRoute] DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        await Sender.Send(command, cancellationToken);

        return NoContent();
    }

    [Authorize(Policy = "OnlyForAdmin")]
    [HttpDelete("images/{id}")]
    public async Task<IActionResult> DeleteImage(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductImageCommand(id);
        await Sender.Send(command, cancellationToken);

        return NoContent();
    }
}
