using Application.Abstractions;
using Application.Abstractions.Messaging;
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
    private readonly IGuestCartService _guestCartService;
    private readonly ICurrentUserService _currentUserService;

    public RegisterCommandHandler(
        IRepository<User> userRepository, 
        IRepository<Cart> cartRepository, 
        IUnitOfWork unitOfWork, 
        IPasswordHasher passwordHasher,
        IGuestCartService guestCartService,
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
        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        var user = new User(
            request.Email, 
            hashedPassword, 
            Enum.Parse<UserRole>(request.Role, true));

        await _userRepository.AddAsync(user, cancellationToken);

        // Transfer the guest cart, or create a new one
        var guestCartId = _currentUserService.GuestCartId;
        var guestCart = await _guestCartService.GetCartAsync(guestCartId);

        Cart cart;
        if (guestCart != null)
        {
            cart = new Cart(guestCart.Id, user.Id);

            foreach (var item in guestCart.Items)
            {  
                cart.Items.Add(item.Adapt<CartItem>());
            }

            await _guestCartService.DeleteCartAsync(guestCartId);
        }
        else
        {
            cart = new Cart(user.Id);
        }

        await _cartRepository.AddAsync(cart, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
