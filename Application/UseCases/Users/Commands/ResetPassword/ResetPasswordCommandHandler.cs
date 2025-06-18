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
        var userId = await _userRepository.GetAsync<EntityIdDto>(
            filter: new UserFilter { Email = request.Email }, 
            asNoTracking: true,
            cancellationToken);

        if (userId == null)
        {
            throw new NoUserWithSuchEmailException(request.Email);
        }    

        var resetToken = await _resetTokenRepository.GetAsync(
            filter: new ResetTokenFilter { UserId = userId, Token = request.ResetToken },
            asNoTracking: false,
            cancellationToken);

        if (resetToken == null)
            throw new InvalidResetTokenException();

        if (resetToken.ExpiresOnUtc < DateTime.UtcNow)
            throw new ExpiredResetTokenException();

        var user = await _userRepository.GetAsync(
            filter: new UserFilter { Id = userId },
            asNoTracking: false,
            cancellationToken);

        if (user == null)
        {
            throw new NotFoundByIdException<User>(userId);
        }

        user.PasswordHash = _passwordHasher.HashPassword(request.Password);
        _userRepository.Update(user);

        await _resetTokenRepository.RemoveAsync(
            filter: new ResetTokenFilter { UserId = user.Id},
            cancellationToken);

        await _unitOfWork.SaveChangesAsync();
    }
}
