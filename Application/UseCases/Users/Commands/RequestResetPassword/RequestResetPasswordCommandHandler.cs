using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Dtos;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using FluentEmail.Core;

namespace Application.UseCases.Users.Commands.RequestResetPassword;

public class RequestResetPasswordCommandHandler : ICommandHandler<RequestResetPasswordCommand>
{
    private readonly IFluentEmail _fluetEmail;
    private readonly ITokenProvider _tokenProvider;
    private readonly IRepository<ResetToken> _resetTokenRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RequestResetPasswordCommandHandler(
        IFluentEmail fluetEmail, 
        ITokenProvider tokenProvider,
        IRepository<ResetToken> resetTokenRepository,
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _fluetEmail = fluetEmail;
        _tokenProvider = tokenProvider;
        _resetTokenRepository = resetTokenRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RequestResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // Retrieve UserId for token (email existense validated)
        var userId = await _userRepository.GetAsync<EntityIdDto>(
            filter: new UserFilter { Email = request.Email },
            cancellationToken);

        // Remove old token and add new one
        string token = _tokenProvider.GenerateResetToken();
        ResetToken resetToken = new(
           Guid.NewGuid(),
           userId.Id,
           token);

        await _resetTokenRepository.RemoveAsync(
            filter: new ResetTokenFilter { UserId = userId.Id });

        await _resetTokenRepository.AddAsync(resetToken);

        // Send Email with token
        await _fluetEmail
            .To(request.Email)
            .Subject("Reset password")
            .Body(token)
            .SendAsync(cancellationToken);

        await _unitOfWork.SaveChangesAsync();
    }
}
