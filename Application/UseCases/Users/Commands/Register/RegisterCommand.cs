using Application.Abstractions.Messaging;

namespace Application.UseCases.Users.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    string Role) : ICommand<Guid>;
