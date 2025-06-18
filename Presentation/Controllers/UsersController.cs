using Application.UseCases.Users.Commands.Login;
using Application.UseCases.Users.Commands.Register;
using Application.UseCases.Users.Commands.RequestResetPassword;
using Application.UseCases.Users.Commands.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class UsersController : ApiController
{
    public UsersController(ISender sender) : base(sender)
    {
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command);

        return Created(string.Empty, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command);

        return Ok(result);
    }

    [HttpPost("password-reset")]
    public async Task<IActionResult> RequestResetPassword(
        [FromBody] RequestResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await Sender.Send(command);

        return Ok();
    }
    [HttpPost("password-reset/confirm")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await Sender.Send(command);

        return Ok();
    }
}

