using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class ExpiredRefreshTokenException : BadRequestException
{
    public ExpiredRefreshTokenException()
        : base("Expired refresh token") { }
}
