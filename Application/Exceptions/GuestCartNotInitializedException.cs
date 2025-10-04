using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class GuestCartNotInitializedException : NotFoundException
{
    public GuestCartNotInitializedException() 
        : base("Guest cart not found")
    {
    }
}
