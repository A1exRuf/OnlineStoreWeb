using Application.Abstractions;
using Application.Dtos.Cart;
using Application.Dtos.CartItem;
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
    private readonly Mock<IGuestCartService> _guestCartServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IRepository<User>>();
        _cartRepositoryMock = new Mock<IRepository<Cart>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _guestCartServiceMock = new Mock<IGuestCartService>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _handler = new RegisterCommandHandler(
            _userRepositoryMock.Object,
            _cartRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _guestCartServiceMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CreateUserAndEmptyCart_WhenGuestCartIsNull()
    {
        // Arrange
        var command = new RegisterCommand("email@test.com", "password", "password", UserRole.Customer.ToString());
        var expectedHashedPassword = "hashedPassword";
        var userId = Guid.NewGuid();

        _passwordHasherMock
            .Setup(p => p.HashPassword(command.Password))
            .Returns(expectedHashedPassword);

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => u.GetType().GetProperty("Id")!.SetValue(u, userId));

        _currentUserServiceMock.Setup(x => x.GuestCartId).Returns(Guid.NewGuid());
        _guestCartServiceMock.Setup(s => s.GetCartAsync(It.IsAny<Guid>())).ReturnsAsync((GuestCartDto?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        _cartRepositoryMock.Verify(r =>
            r.AddAsync(It.Is<Cart>(c => c.UserId == result && !c.Items.Any()), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(userId, result);
    }

    [Fact]
    public async Task Handle_Should_CreateUserAndTransferGuestCart_WhenGuestCartExists()
    {
        // Arrange
        var command = new RegisterCommand("email@test.com", "password", "password", UserRole.Customer.ToString());
        var expectedHashedPassword = "hashedPassword";
        var guestCartId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var guestCartDto = new GuestCartDto(
            guestCartId,
            new List<GuestCartItemDto>
            {
            new GuestCartItemDto(Guid.NewGuid(), Guid.NewGuid(), 3)
            });

        _passwordHasherMock
            .Setup(p => p.HashPassword(command.Password))
            .Returns(expectedHashedPassword);

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => u.GetType().GetProperty("Id")!.SetValue(u, userId));

        _currentUserServiceMock.Setup(x => x.GuestCartId).Returns(guestCartId);
        _guestCartServiceMock.Setup(s => s.GetCartAsync(guestCartId)).ReturnsAsync(guestCartDto);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        _cartRepositoryMock.Verify(r =>
            r.AddAsync(It.Is<Cart>(c =>
                c.UserId == result &&
                c.Items.Count == guestCartDto.Items.Count &&
                c.Items[0].ProductId == guestCartDto.Items[0].ProductId &&
                c.Items[0].Quantity == guestCartDto.Items[0].Quantity),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _guestCartServiceMock.Verify(s => s.DeleteCartAsync(guestCartId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(userId, result);
    }

    [Fact]
    public async Task Handle_Should_CreateUser_WithCorrectData()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "123456", "123456", UserRole.Customer.ToString());
        var hashed = "hashed123";
        var userId = Guid.NewGuid();

        _passwordHasherMock.Setup(p => p.HashPassword(command.Password)).Returns(hashed);

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => u.GetType().GetProperty("Id")!.SetValue(u, userId));

        _currentUserServiceMock.Setup(s => s.GuestCartId).Returns(Guid.NewGuid());
        _guestCartServiceMock.Setup(s => s.GetCartAsync(It.IsAny<Guid>())).ReturnsAsync((GuestCartDto?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(r => r.AddAsync(
            It.Is<User>(u => u.Email == command.Email && u.PasswordHash == hashed && u.Role == UserRole.Customer),
            It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.Equal(userId, result);
    }

}
