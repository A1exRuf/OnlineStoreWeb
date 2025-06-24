using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;

namespace Application.UseCases.Users.Commands.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, Guid>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(
        IRepository<User> userRepository, 
        IRepository<Cart> cartRepository, 
        IUnitOfWork unitOfWork, 
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        var user = new User(
            request.Email, 
            hashedPassword, 
            Enum.Parse<UserRole>(request.Role, true));

        await _userRepository.AddAsync(user, cancellationToken);

        var cart = new Cart(
            Guid.NewGuid(),
            user.Id);

        await _cartRepository.AddAsync(cart, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
