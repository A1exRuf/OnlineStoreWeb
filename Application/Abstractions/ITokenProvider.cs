using Domain.Entities;

namespace Application.Abstractions;

public interface ITokenProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string GenerateResetToken();
}
