using Application.Abstractions;
using Application.Exceptions;
using Application.Filters;
using Application.UseCases.Users.Commands.Login;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    private readonly Mock<IRepository<RefreshToken>> _refreshTokenRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IRepository<User>>();    
        _refreshTokenRepositoryMock = new Mock<IRepository<RefreshToken>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenProviderMock = new Mock<ITokenProvider>();

        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _tokenProviderMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnLoginResponse_When_CredentialsAreValid()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var passwordHash = "hashedPassword";
        var accessToken = "access-token";
        var refreshTokenValue = "refresh-token";

        var user = new User(email, passwordHash, UserRole.Customer);


        _userRepositoryMock.Setup(r => r.GetAsync(
            It.IsAny<UserFilter>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(p => p.VerifyPassword(passwordHash, password)).Returns(true);
        _tokenProviderMock.Setup(t => t.GenerateAccessToken(user)).Returns(accessToken);
        _tokenProviderMock.Setup(t => t.GenerateRefreshToken()).Returns(refreshTokenValue);

        var command = new LoginCommand(email, password);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        Assert.Equal(accessToken, result.AccessToken);
        Assert.Equal(refreshTokenValue, result.RefreshToken);

        _refreshTokenRepositoryMock.Verify(r => r.RemoveAsync(It.IsAny<RefreshTokenFilter>(), default), Times.Once);
        _refreshTokenRepositoryMock.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowInvalidEmailOrPasswordException_When_UserNotFound()
    {
        // Arrange
        var command = new LoginCommand("notfound@example.com", "password");

        _userRepositoryMock.Setup(r => r.GetAsync(
            It.IsAny<UserFilter>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidEmailOrPasswordException>(() =>
            _handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_Should_ThrowInvalidEmailOrPasswordException_When_PasswordIsIncorrect()
    {
        // Arrange
        var email = "test@example.com";
        var correctHash = "hashedPassword";
        var wrongPassword = "wrong-password";
        var user = new User(email, correctHash, UserRole.Customer);

        _userRepositoryMock.Setup(r => r.GetAsync(
            It.IsAny<UserFilter>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(p => p.VerifyPassword(correctHash, wrongPassword)).Returns(false);

        var command = new LoginCommand(email, wrongPassword);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidEmailOrPasswordException>(() =>
            _handler.Handle(command, default));
    }
}
