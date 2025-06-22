using Application.UseCases.Products.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class ProductsController : ApiController
{
    public ProductsController(ISender sender) : base(sender)
    {
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
