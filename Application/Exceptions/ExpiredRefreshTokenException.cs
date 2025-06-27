using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class ExpiredRefreshTokenException : UnauthorizedException
{
    public ExpiredRefreshTokenException()
        : base("Expired refresh token") { }
}
