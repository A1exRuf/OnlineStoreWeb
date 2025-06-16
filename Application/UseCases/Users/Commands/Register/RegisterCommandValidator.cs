using Application.Filters;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Users.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(IRepository<User> repository)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MustAsync(async (email, cancellationToken) =>
            {
                return !await repository.ExistsAsync(
                    new UserFilter { Email = email }, cancellationToken);
            }).WithMessage("The email is already taken");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match"); ;

        RuleFor(x => x.Role)
            .Must(role => Enum.TryParse<UserRole>(role, true, out _))
            .WithMessage("Invalid role");
    }
}
