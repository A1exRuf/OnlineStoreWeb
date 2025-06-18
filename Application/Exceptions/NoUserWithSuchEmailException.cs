using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class NoUserWithSuchEmailException : BadRequestException
{
    public NoUserWithSuchEmailException(string email) 
        : base($"User with email: {email} does not exists")
    {
    }
}
