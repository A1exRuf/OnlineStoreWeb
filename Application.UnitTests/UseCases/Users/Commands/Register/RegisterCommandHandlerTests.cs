using Application.Abstractions;
using Application.Abstractions.Users.CartInitialization;
using Application.UseCases.Users.Commands.Register;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.UnitTests.UseCases.Users.Commands.Register;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ICartInitializationStrategyFactory> _cartFactoryMock;
    private readonly Mock<ICartInitializationStrategy> _cartStrategyMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IRepository<User>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _cartFactoryMock = new Mock<ICartInitializationStrategyFactory>();
        _cartStrategyMock = new Mock<ICartInitializationStrategy>();

        _cartFactoryMock
            .Setup(f => f.CreateAsync())
            .ReturnsAsync(_cartStrategyMock.Object);

        _handler = new RegisterCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _cartFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CreateUserAndCart_AndReturnUserId_When_CommandIsValid()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "123456",
            ConfirmPassword: "123456",
            Role: "Customer");

        var hashedPassword = "hashed_12345";
        var cartId = Guid.NewGuid();

        _passwordHasherMock
            .Setup(h => h.HashPassword(command.Password))
            .Returns(hashedPassword);

        _cartStrategyMock
            .Setup(s => s.InitializeCartAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) =>
            {
                user.Cart = new Cart(cartId);
            })
            .Returns(Task.CompletedTask);

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) =>
            {
                typeof(User).GetProperty(nameof(User.Id))!.SetValue(user, Guid.NewGuid());
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        _passwordHasherMock.Verify(h => h.HashPassword(command.Password), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAsync(
            It.Is<User>(u =>
                u.Email == command.Email &&
                u.Role == UserRole.Customer &&
                u.Cart != null),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_UseCartInitializationStrategy_When_CreatingUser()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "123456",
            ConfirmPassword: "123456",
            Role: "Customer");

        _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hash");
        _cartStrategyMock
            .Setup(s => s.InitializeCartAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _cartFactoryMock.Verify(f => f.CreateAsync(), Times.Once);
        _cartStrategyMock.Verify(s => s.InitializeCartAsync(
            It.Is<User>(u => u.Email == command.Email),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_CallSaveChangesOnce_When_RegistrationCompletesSuccessfully()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "123456",
            ConfirmPassword: "123456",
            Role: "Customer");

        _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");
        _cartStrategyMock.Setup(s => s.InitializeCartAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Invocations.Should()
            .ContainSingle(i => i.Method.Name == nameof(IUnitOfWork.SaveChangesAsync));
    }

    [Fact]
    public async Task Handle_Should_PassCancellationTokenToDependencies_When_TokenIsProvided()
    {
        // Arrange
        var token = new CancellationToken();

        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "123456",
            ConfirmPassword: "123456",
            Role: "Customer");

        _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hash");
        _cartStrategyMock
            .Setup(s => s.InitializeCartAsync(It.IsAny<User>(), token))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, token);

        // Assert
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), token), Times.Once);
        _cartFactoryMock.Verify(f => f.CreateAsync(), Times.Once);
        _cartStrategyMock.Verify(s => s.InitializeCartAsync(It.IsAny<User>(), token), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(token), Times.Once);
    }
}