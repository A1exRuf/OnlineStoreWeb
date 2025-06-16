using Application.Filters;
using Application.UseCases.Users.Commands.Register;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using FluentValidation.TestHelper;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;
    private readonly Mock<IRepository<User>> _repositoryMock;

    public RegisterCommandValidatorTests()
    {
        _repositoryMock = new Mock<IRepository<User>>();
        _validator = new RegisterCommandValidator(_repositoryMock.Object);
    }

    [Theory]
    [InlineData("", "Email is required")]
    [InlineData("invalid-email", "Invalid email format")]
    [InlineData("email@taken.com", "The email is already taken", true)]
    public async Task Should_HaveValidationError_When_EmailIsInvalid(
        string email, 
        string expectedErrorMessage, 
        bool emailTaken = false)
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.ExistsAsync(
                It.Is<UserFilter>(f => f.Email == email),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emailTaken);

        var command = new RegisterCommand(email, "password", "password", UserRole.Customer.ToString());
        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage(expectedErrorMessage);
    }

    [Theory]
    [InlineData("", "Password is required")]
    [InlineData("1234567", "Password must be at least 8 characters long")]
    public async Task Should_HaveValidationError_When_PasswordIsInvalid(string password, string expectedErrorMessage)
    {
        // Arrange
        var command = new RegisterCommand("email@test.com", password, password, UserRole.Customer.ToString());
        // Act
        var result = await _validator.TestValidateAsync(command);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage(expectedErrorMessage);
    }

    [Fact]
    public async Task Should_HaveValidationError_When_ConfirmPasswordIsInvalid()
    {
        // Arrange
        var command = new RegisterCommand("email@test.com", "password", "12345678", UserRole.Customer.ToString());
        // Act
        var result = await _validator.TestValidateAsync(command);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword).WithErrorMessage("Passwords do not match");
    }

    [Fact]
    public async Task Should_HaveValidationError_When_RoleIsInvalid()
    {
        // Arrange
        var command = new RegisterCommand("email@test.com", "password", "password", "TaxiDriver");
        // Act
        var result = await _validator.TestValidateAsync(command);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Role).WithErrorMessage("Invalid role");
    }

    [Theory]
    [InlineData("customer")]
    [InlineData("cUstomer")]
    [InlineData("admin")]
    [InlineData("AdmiN")]
    public async Task Should_HaveNoValidationError_When_RoleIsValid(string role)
    {
        // Arrange
        var command = new RegisterCommand("email@test.com", "password", "password", role);
        // Act
        var result = await _validator.TestValidateAsync(command);
        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }

    [Fact]
    public async Task Should_NotHaveAnyValidationErrors_When_CommandIsValid()
    {
        var command = new RegisterCommand("email@test.com", "password", "password", UserRole.Customer.ToString());

        _repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<UserFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
