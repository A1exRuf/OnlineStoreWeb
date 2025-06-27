using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class InvalidResetTokenException : UnauthorizedException
{
    public InvalidResetTokenException()
        : base("Invalid reset token") {}
}
