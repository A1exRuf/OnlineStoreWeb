using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Users.Commands.RequestResetPassword;

public class RequestResetPasswordValidator : AbstractValidator<RequestResetPasswordCommand>
{
    public RequestResetPasswordValidator(IRepository<User> repository)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MustAsync(async (email, cancellationToken) =>
            {
                return await repository.ExistsAsync(
                    new UserFilter { Email = email }, cancellationToken);
            }).WithMessage("This Email doesn't exist");
    }
}
