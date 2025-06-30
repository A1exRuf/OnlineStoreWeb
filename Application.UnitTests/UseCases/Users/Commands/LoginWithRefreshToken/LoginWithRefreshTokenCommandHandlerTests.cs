using Application.Abstractions;
using Application.Exceptions;
using Application.UseCases.Users.Commands.LoginWithRefreshToken;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands.LoginWithRefreshToken;

public class LoginWithRefreshTokenCommandHandlerTests
{
    private readonly Mock<IRepository<RefreshToken>> _refreshTokenRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly LoginWithRefreshTokenCommandHandler _handler;

    public LoginWithRefreshTokenCommandHandlerTests()
    {
        _refreshTokenRepositoryMock = new Mock<IRepository<RefreshToken>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tokenProviderMock = new Mock<ITokenProvider>();

        _handler = new LoginWithRefreshTokenCommandHandler(
            _refreshTokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tokenProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTokens_WhenRefreshTokenIsValid()
    {
        // Arrange
        var user = new User("test@example.com", "hash", UserRole.Customer);
        var refreshToken = new RefreshToken("valid-token", user.Id);

        typeof(RefreshToken).GetProperty(nameof(RefreshToken.User))!
            .SetValue(refreshToken, user);
        typeof(RefreshToken).GetProperty(nameof(RefreshToken.ExpiresOnUtc))!
            .SetValue(refreshToken, DateTime.UtcNow.AddMinutes(10));

        _refreshTokenRepositoryMock
            .Setup(r => r.GetAsync(
                It.IsAny<IFilter<RefreshToken>>(),
                false,
                It.IsAny<CancellationToken>(),
                new[] { "User" }))
            .ReturnsAsync(refreshToken);

        _tokenProviderMock
            .Setup(p => p.GenerateAccessToken(user))
            .Returns("access-token");

        _tokenProviderMock
            .Setup(p => p.GenerateRefreshToken())
            .Returns("new-refresh-token");

        // Act
        var result = await _handler.Handle(
            new LoginWithRefreshTokenCommand("valid-token"),
            CancellationToken.None);

        // Assert
        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);

        _refreshTokenRepositoryMock.Verify(r => r.Update(refreshToken), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowExpiredRefreshTokenException_WhenTokenIsExpired()
    {
        // Arrange
        var user = new User("test@example.com", "hash", UserRole.Customer);
        var refreshToken = new RefreshToken("expired-token", user.Id);

        typeof(RefreshToken).GetProperty(nameof(RefreshToken.User))!
            .SetValue(refreshToken, user);
        typeof(RefreshToken).GetProperty(nameof(RefreshToken.ExpiresOnUtc))!
            .SetValue(refreshToken, DateTime.UtcNow.AddMinutes(-10));

        _refreshTokenRepositoryMock
            .Setup(r => r.GetAsync(
                It.IsAny<IFilter<RefreshToken>>(),
                false,
                It.IsAny<CancellationToken>(),
                new[] { "User" }))
            .ReturnsAsync(refreshToken);

        // Act & Assert
        await Assert.ThrowsAsync<ExpiredRefreshTokenException>(() =>
            _handler.Handle(new LoginWithRefreshTokenCommand("expired-token"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowExpiredRefreshTokenException_WhenTokenIsNotFound()
    {
        // Arrange
        _refreshTokenRepositoryMock
            .Setup(r => r.GetAsync(
                It.IsAny<IFilter<RefreshToken>>(),
                false,
                It.IsAny<CancellationToken>(),
                new[] { "User" }))
            .ReturnsAsync((RefreshToken?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ExpiredRefreshTokenException>(() =>
            _handler.Handle(new LoginWithRefreshTokenCommand("nonexistent-token"), CancellationToken.None));
    }
}
