using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Exceptions;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.UseCases.Users.Commands.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;

    public LoginCommandHandler(
        IRepository<User> userRepository, 
        IRepository<RefreshToken> refreshTokenRepository, 
        IUnitOfWork unitOfWork, 
        IPasswordHasher passwordHasher, 
        ITokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // User verification
        var userFilter = new UserFilter { Email = request.Email };

        var user = await  _userRepository.GetAsync(
            userFilter,
            cancellationToken: cancellationToken);

        if (user == null) 
            throw new InvalidEmailOrPasswordException();

        bool passwordVerified = _passwordHasher.VerifyPassword(user.PasswordHash, request.Password);

        if (!passwordVerified)
            throw new InvalidEmailOrPasswordException();

        // Building tokens
        var acсessToken = _tokenProvider.GenerateAccessToken(user);
        var refreshToken = new RefreshToken(
            Guid.NewGuid(),
            _tokenProvider.GenerateRefreshToken(),
            user.Id);

        var refreshTokenFilter = new RefreshTokenFilter { UserId = user.Id };
        await _refreshTokenRepository.RemoveAsync(refreshTokenFilter, cancellationToken);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new LoginResponse(acсessToken, refreshToken.Token);

        return response;
    }
}
