using Application.Filters;
using Application.UseCases.Users.Commands.RequestResetPassword;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation.TestHelper;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands.RequestResetPassword;

public class RequestResetPasswordValidatorTests
{
    private readonly Mock<IRepository<User>> _repositoryMock;
    private readonly RequestResetPasswordValidator _validator;

    public RequestResetPasswordValidatorTests()
    {
        _repositoryMock = new Mock<IRepository<User>>();
        _validator = new RequestResetPasswordValidator(_repositoryMock.Object);
    }

    [Theory]
    [InlineData("", "Email is required")]
    [InlineData("invalid-email", "Invalid email format")]
    [InlineData("nonexistent@email.com", "This Email doesn't exist", false)]
    public async Task Should_HaveValidationError_When_EmailIsInvalid(
        string email,
        string expectedErrorMessage,
        bool emailExists = true)
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.ExistsAsync(
                It.Is<UserFilter>(f => f.Email == email),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emailExists);

        var command = new RequestResetPasswordCommand(email);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage(expectedErrorMessage);
    }
}
