using Domain.Exceptions.Base;

namespace Domain.Exceptions;

public class NonPositiveIncreaseAmountException : BadRequestException
{
    public NonPositiveIncreaseAmountException()
        : base("Increase amount must be positive")
    {
    }
}
