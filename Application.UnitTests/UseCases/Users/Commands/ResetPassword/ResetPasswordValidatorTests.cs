using Application.UseCases.Users.Commands.ResetPassword;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands.ResetPassword;

public class ResetPasswordValidatorTests : AbstractValidator<ResetPasswordCommand>
{
    private readonly Mock<IRepository<User>> _repositoryMock;
    private readonly ResetPasswordValidator _validator;

    public ResetPasswordValidatorTests()
    {
        _repositoryMock = new Mock<IRepository<User>>();
        _validator = new ResetPasswordValidator(_repositoryMock.Object);
    }

    [Theory]
    [InlineData("", "Email is required")]
    [InlineData("invalid-email", "Invalid email format")]
    public async Task Should_HaveValidationError_When_EmailIsInvalid(
        string email,
        string expectedErrorMessage)
    {
        // Arrange
        var command = new ResetPasswordCommand(email, "123456", "password", "password");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage(expectedErrorMessage);
    }

    [Fact]
    public async Task Should_HaveValidationError_When_ResetTokenIsEmpty()
    {
        // Arrange
        var command = new ResetPasswordCommand("email@test.com", "", "123456", "123456");

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ResetToken).WithErrorMessage("Reset token is requered");
    }

    [Theory]
    [InlineData("", "Password is required")]
    [InlineData("1234567", "Password must be at least 8 characters long")]
    public async Task Should_HaveValidationError_When_PasswordIsInvalid(string password, string expectedErrorMessage)
    {
        // Arrange
        var command = new ResetPasswordCommand("email@test.com", "123456", password, password);
        // Act
        var result = await _validator.TestValidateAsync(command);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage(expectedErrorMessage);
    }

    [Fact]
    public async Task Should_HaveValidationError_When_ConfirmPasswordIsInvalid()
    {
        // Arrange
        var command = new ResetPasswordCommand("email@test.com", "123456", "password", "12345678");
        // Act
        var result = await _validator.TestValidateAsync(command);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword).WithErrorMessage("Passwords do not match");
    }


}
