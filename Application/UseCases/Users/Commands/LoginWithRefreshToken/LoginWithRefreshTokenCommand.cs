using Application.Abstractions.Messaging;

namespace Application.UseCases.Users.Commands.LoginWithRefreshToken;

public record LoginWithRefreshTokenCommand(string RefreshToken) : ICommand<LoginWithRefreshTokenResponse>;
