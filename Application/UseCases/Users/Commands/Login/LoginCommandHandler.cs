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
        User user = await GetAndVerificateUser(request, cancellationToken);

        LoginResponse response = await GenerateTokens(user, cancellationToken);

        return response;
    }

    private async Task<User> GetAndVerificateUser(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(
            filter: new UserFilter { Email = request.Email },
            cancellationToken: cancellationToken)
        ?? throw new InvalidEmailOrPasswordException();

        bool passwordVerified = _passwordHasher.VerifyPassword(user.PasswordHash, request.Password);

        if (!passwordVerified)
            throw new InvalidEmailOrPasswordException();

        return user;
    }

    private async Task<LoginResponse> GenerateTokens(User user, CancellationToken cancellationToken)
    {
        var acсessToken = _tokenProvider.GenerateAccessToken(user);
        var refreshToken = new RefreshToken(
            _tokenProvider.GenerateRefreshToken(),
            user.Id);

        await _refreshTokenRepository.RemoveAsync(
            filter: new RefreshTokenFilter { UserId = user.Id },
            cancellationToken);

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(acсessToken, refreshToken.Token);
    }
}
