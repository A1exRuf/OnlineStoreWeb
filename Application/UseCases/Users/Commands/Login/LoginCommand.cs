using Application.Abstractions.Messaging;

namespace Application.UseCases.Users.Commands.Login;

public record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;
