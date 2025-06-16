using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class InvalidEmailOrPasswordException : BadRequestException
{
    public InvalidEmailOrPasswordException()
        : base("Invalid email or password") { }
}
