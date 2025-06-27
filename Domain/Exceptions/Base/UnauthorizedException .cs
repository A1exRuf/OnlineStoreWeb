namespace Domain.Exceptions.Base;

public abstract class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) {}
}
