using Application.Abstractions;
using Application.Abstractions.Carts;
using Application.Abstractions.Messaging;
using Application.Dtos.Cart;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using Mapster;

namespace Application.UseCases.Users.Commands.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, Guid>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IGuestCartStorage _guestCartService;
    private readonly ICurrentUserService _currentUserService;

    public RegisterCommandHandler(
        IRepository<User> userRepository, 
        IRepository<Cart> cartRepository, 
        IUnitOfWork unitOfWork, 
        IPasswordHasher passwordHasher,
        IGuestCartStorage guestCartService,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _guestCartService = guestCartService;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        User user = await CreateNewUser(request, cancellationToken);

        await AssignCartToNewUser(user, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    private async Task<User> CreateNewUser(RegisterCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        var user = new User(
            request.Email,
            hashedPassword,
            Enum.Parse<UserRole>(request.Role, true));

        await _userRepository.AddAsync(user, cancellationToken);
        return user;
    }

    private async Task AssignCartToNewUser(User user, CancellationToken cancellationToken)
    {
        var guestCartId = _currentUserService.GuestCartId;
        var guestCart = await _guestCartService.GetCartAsync();

        Cart cart;

        if (guestCart != null)
            cart = await TransferGuestCart(user, guestCartId, guestCart);
        else
            cart = new Cart(user.Id);

        await _cartRepository.AddAsync(cart, cancellationToken);
    }

    private async Task<Cart> TransferGuestCart(User user, Guid guestCartId, GuestCartDto guestCart)
    {
        Cart cart = new Cart(guestCart.Id, user.Id);

        cart.Items.AddRange(guestCart.Items.Adapt<List<CartItem>>());

        await _guestCartService.DeleteCartAsync();

        return cart;
    }
}
