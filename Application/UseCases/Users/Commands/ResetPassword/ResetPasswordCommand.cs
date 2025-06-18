using Application.Abstractions.Messaging;

namespace Application.UseCases.Users.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email, 
    string ResetToken, 
    string Password, 
    string ConfirmPassword) : ICommand;
