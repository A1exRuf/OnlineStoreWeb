using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Exceptions;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.UseCases.Users.Commands.LoginWithRefreshToken;

public class LoginWithRefreshTokenCommandHandler : ICommandHandler<LoginWithRefreshTokenCommand, LoginWithRefreshTokenResponse>
{
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenProvider _tokenProvider;

    public LoginWithRefreshTokenCommandHandler(
        IRepository<RefreshToken> refreshTokenRepository, 
        IUnitOfWork unitOfWork, 
        ITokenProvider tokenProvider)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _tokenProvider = tokenProvider;
    }

    public async Task<LoginWithRefreshTokenResponse> Handle(LoginWithRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        RefreshToken? refreshToken = await _refreshTokenRepository.GetAsync(
            filter: new RefreshTokenFilter { Token = request.RefreshToken },
            asNoTracking: false,
            cancellationToken,
            rt => rt.User);

        if (refreshToken == null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
            throw new ExpiredRefreshTokenException();

        string accessToken = _tokenProvider.GenerateAccessToken(refreshToken.User);

        refreshToken.UpdateToken(_tokenProvider.GenerateRefreshToken());

        _refreshTokenRepository.Update(refreshToken);

        await _unitOfWork.SaveChangesAsync();

        return new LoginWithRefreshTokenResponse(accessToken, refreshToken.Token);
    }
}
