using Application.UseCases.Categories.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class CategoriesController : ApiController
{
    public CategoriesController(ISender sender) : base(sender)
    {
    }

    [Authorize(Policy = "OnlyForAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command);

        return Created(string.Empty, result); // Insert URI!!!
    }
}
