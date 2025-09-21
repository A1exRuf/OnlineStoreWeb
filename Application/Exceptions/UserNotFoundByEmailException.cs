using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class UserNotFoundByEmailException : BadRequestException
{
    public UserNotFoundByEmailException(string email) : base($"User with email: {email} does not exist")
    {
    }
}
