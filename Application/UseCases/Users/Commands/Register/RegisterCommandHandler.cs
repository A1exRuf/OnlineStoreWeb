using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Abstractions.Users.CartInitialization;
using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;

namespace Application.UseCases.Users.Commands.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, Guid>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICartInitializationStrategyFactory _cartInitializationFactory;

    public RegisterCommandHandler(
        IRepository<User> userRepository, 
        IUnitOfWork unitOfWork, 
        IPasswordHasher passwordHasher,
        ICartInitializationStrategyFactory cartInitializationStrategyFactory)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _cartInitializationFactory = cartInitializationStrategyFactory;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        var user = new User(
            request.Email,
            hashedPassword,
            Enum.Parse<UserRole>(request.Role, true));

        // Transfers guest cart if exists
        var strategy = await _cartInitializationFactory.CreateAsync();
        await strategy.InitializeCartAsync(user, cancellationToken);

        await _userRepository.AddAsync(user, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
