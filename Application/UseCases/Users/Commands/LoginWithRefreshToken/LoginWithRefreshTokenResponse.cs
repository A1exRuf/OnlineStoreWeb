namespace Application.UseCases.Users.Commands.LoginWithRefreshToken;

public record LoginWithRefreshTokenResponse(
    string AccessToken,
    string RefreshToken);

