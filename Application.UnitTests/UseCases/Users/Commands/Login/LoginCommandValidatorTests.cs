using Application.UseCases.Users.Commands.Login;
using FluentValidation.TestHelper;

namespace Application.UnitTests.UseCases.Users.Commands.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public async Task Should_HaveValidationError_When_EmailIsEmpty()
    {
        // Arrange
        var command = new LoginCommand("", "password");
        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email is required");
    }

    [Fact]
    public async Task Should_HaveValidationError_When_PasswordIsEmpty()
    {
        // Arrange
        var command = new LoginCommand("email@test.com", "");
        // Act
        var result = await _validator.TestValidateAsync(command);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage("Password is required");
    }
}
