using Application.UseCases.Categories.Commands.Create;
using Application.UseCases.Categories.Queries.GetCategoriesQuery;
using Application.UseCases.Categories.Queries.GetCategoryByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class CategoriesController : ApiController
{
    public CategoriesController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery();
        var result = await Sender.Send(query);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await Sender.Send(query);

        return Ok(result);
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
