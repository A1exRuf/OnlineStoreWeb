using Domain.Exceptions.Base;

namespace Application.Exceptions;

public class ValidationException : BadRequestException
{
    public ValidationException(Dictionary<string, string[]> errors) 
        : base("Validation errors occured") => Errors = errors;

    public Dictionary<string, string[]> Errors { get; }
}
