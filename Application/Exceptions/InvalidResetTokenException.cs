using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class InvalidResetTokenException : BadRequestException
{
    public InvalidResetTokenException()
        : base("Invalid reset token") {}
}
