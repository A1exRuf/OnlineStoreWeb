using Application.Abstractions.Messaging;

namespace Application.UseCases.Users.Commands.RequestResetPassword;

public record RequestResetPasswordCommand(string Email) : ICommand;
