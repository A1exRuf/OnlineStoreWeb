using Application.Abstractions;
using Application.UseCases.Users.Commands.Register;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands.Register;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    private readonly Mock<IRepository<Cart>> _cartRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IRepository<User>>();
        _cartRepositoryMock = new Mock<IRepository<Cart>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _handler = new RegisterCommandHandler(
            _userRepositoryMock.Object,
            _cartRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CreateUser_WhenValidCommand()
    {
        // Arrange
        RegisterCommand command = new(
            "email@test.com",
            "password",
            "password",
            UserRole.Customer.ToString());

        var expectedHashedPassword = "hashedPassword";

        _passwordHasherMock
            .Setup(p => p.HashPassword(command.Password))
            .Returns(expectedHashedPassword);

        // Act
        Guid result = await _handler.Handle(command, default);

        // Assert
        _userRepositoryMock.Verify(
            repos => repos.AddAsync(
                It.Is<User>(u =>
                u.Email == command.Email &&
                u.PasswordHash == expectedHashedPassword &&
                u.Role == Enum.Parse<UserRole>(command.Role, true)),
                It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.NotEqual(Guid.Empty, result);
    }
}
