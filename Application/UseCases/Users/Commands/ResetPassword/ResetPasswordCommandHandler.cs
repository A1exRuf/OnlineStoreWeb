using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Dtos;
using Application.Exceptions;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<ResetToken> _resetTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(
        IRepository<User> userRepository, 
        IRepository<ResetToken> resetTokenRepository, 
        IUnitOfWork unitOfWork,
        IPasswordHasher password)
    {
        _userRepository = userRepository;
        _resetTokenRepository = resetTokenRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = password;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        Guid userId = await GetUserId(request.Email, cancellationToken);
        await ValidateAndRemoveResetToken(request, userId, cancellationToken);
        await ChangePassword(request, userId, cancellationToken);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Guid> GetUserId(string email, CancellationToken cancellationToken)
    {
        return await _userRepository.GetAsync<EntityIdDto>(
            filter: new UserFilter { Email = email },
            cancellationToken)
        ?? throw new UserNotFoundByEmailException(email);
    }

    private async Task ValidateAndRemoveResetToken(ResetPasswordCommand request, Guid userId, CancellationToken cancellationToken)
    {
        var resetToken = await _resetTokenRepository.GetAsync(
            filter: new ResetTokenFilter { UserId = userId, Token = request.ResetToken },
            cancellationToken);

        if (resetToken == null || resetToken.ExpiresOnUtc < DateTime.UtcNow)
            throw new InvalidResetTokenException();

        await _resetTokenRepository.RemoveAsync(
            filter: new ResetTokenFilter { UserId = userId },
            cancellationToken);
    }

    private async Task ChangePassword(ResetPasswordCommand request, Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(
            filter: new UserFilter { Id = userId },
            cancellationToken)
        ?? throw new NotFoundByIdException<User>(userId);

        user.PasswordHash = _passwordHasher.HashPassword(request.Password);
        _userRepository.Update(user);
    }
}
