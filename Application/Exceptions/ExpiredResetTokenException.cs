using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class ExpiredResetTokenException : BadRequestException
{
    public ExpiredResetTokenException() 
        : base("Expired reset token") {}
}
