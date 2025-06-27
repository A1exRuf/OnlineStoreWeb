using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class UserNotAuthenticatedException : UnauthorizedException
{
    public UserNotAuthenticatedException() 
        : base("User is not authenticated") {}
}
