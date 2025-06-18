using Application.Abstractions;
using Application.Dtos;
using Application.UseCases.Users.Commands.RequestResetPassword;
using Domain.Abstractions;
using Domain.Entities;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands.RequestResetPassword;

public class RequestResetPasswordHandlerTests
{
    private readonly Mock<IRepository<ResetToken>> _resetTokenRepositoryMock;
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly Mock<IFluentEmail> _fluentEmailMock;
    private readonly RequestResetPasswordCommandHandler _handler;

    public RequestResetPasswordHandlerTests()
    {
        _resetTokenRepositoryMock = new Mock<IRepository<ResetToken>>();
        _userRepositoryMock = new Mock<IRepository<User>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tokenProviderMock = new Mock<ITokenProvider>();
        _fluentEmailMock =  new Mock<IFluentEmail>();

        _handler = new RequestResetPasswordCommandHandler(
            _fluentEmailMock.Object,
            _tokenProviderMock.Object,
            _resetTokenRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldRequestPasswordResetSuccessfully_CommandIsValid()
    {
        // Arrange
        var email = "email@test.com";
        var userId = Guid.NewGuid();
        var token = "123456";
        var command = new RequestResetPasswordCommand(email);

        _userRepositoryMock
            .Setup(x => x.GetAsync<EntityIdDto>(
                It.IsAny<IFilter<User>>(),
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EntityIdDto(userId));

        _tokenProviderMock
            .Setup(x => x.GenerateResetToken())
            .Returns(token);

        _fluentEmailMock
            .Setup(x => x.To(email))
            .Returns(_fluentEmailMock.Object);

        _fluentEmailMock
            .Setup(x => x.Subject(It.IsAny<string>()))
            .Returns(_fluentEmailMock.Object);

        _fluentEmailMock
            .Setup(x => x.Body(It.IsAny<string>(), default))
            .Returns(_fluentEmailMock.Object);

        _fluentEmailMock
            .Setup(x => x.SendAsync(CancellationToken.None))
            .ReturnsAsync(new SendResponse
            {
                ErrorMessages = new List<string>()
            });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(x => x.GetAsync<EntityIdDto>(
            It.IsAny<IFilter<User>>(), true, It.IsAny<CancellationToken>()), Times.Once);

        _resetTokenRepositoryMock.Verify(x => x.RemoveAsync(
            It.IsAny<IFilter<ResetToken>>(), It.IsAny<CancellationToken>()), Times.Once);

        _resetTokenRepositoryMock.Verify(x => x.AddAsync(
            It.Is<ResetToken>(rt => rt.UserId == userId && rt.Token == token),
            It.IsAny<CancellationToken>()), Times.Once);

        _fluentEmailMock.Verify(x => x.SendAsync(It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
