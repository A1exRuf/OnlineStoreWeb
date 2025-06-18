using Application.Abstractions;
using Application.Dtos;
using Application.UseCases.Users.Commands.ResetPassword;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IRepository<User>> _userRepoMock;
    private readonly Mock<IRepository<ResetToken>> _tokenRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _hasherMock;
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _userRepoMock = new Mock<IRepository<User>>();
        _tokenRepoMock = new Mock<IRepository<ResetToken>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _hasherMock = new Mock<IPasswordHasher>();

        _handler = new ResetPasswordCommandHandler(
            _userRepoMock.Object,
            _tokenRepoMock.Object,
            _unitOfWorkMock.Object,
            _hasherMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldResetPassword_WhenCommandIsValid()
    {
        // Arrange
        var email = "email@test.com";
        var userId = Guid.NewGuid();
        var newPassword = "new_password";
        var hashedPassword = "hashed_password";
        var token = "valid_token";

        var command = new ResetPasswordCommand(
            email,
            token,
            newPassword,
            newPassword);

        _userRepoMock
            .Setup(r => r.GetAsync<EntityIdDto>(It.IsAny<IFilter<User>>(), true, default))
            .ReturnsAsync(new EntityIdDto(userId));

        _tokenRepoMock
            .Setup(r => r.GetAsync(It.IsAny<IFilter<ResetToken>>(), false, default))
            .ReturnsAsync(new ResetToken(Guid.NewGuid(), userId, token));

        var user = new User(userId, email, "old_hash", UserRole.Customer);

        _userRepoMock
            .Setup(r => r.GetAsync(It.IsAny<IFilter<User>>(), false, default))
            .ReturnsAsync(user);

        _hasherMock
            .Setup(h => h.HashPassword(newPassword))
            .Returns(hashedPassword);

        // Act
        await _handler.Handle(command, default);

        // Assert
        Assert.Equal(hashedPassword, user.PasswordHash);
        _userRepoMock.Verify(r => r.Update(user), Times.Once);
        _tokenRepoMock.Verify(r => r.RemoveAsync(It.IsAny<IFilter<ResetToken>>(), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
