using Domain.Exceptions.Base;

namespace Domain.Exceptions;

public class NonPositiveQuantityException : BadRequestException
{
    public NonPositiveQuantityException() 
        : base($"Quantity must be positive")
    {
    }
}
