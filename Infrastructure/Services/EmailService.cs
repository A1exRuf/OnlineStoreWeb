using Application.Abstractions;
using FluentEmail.Core;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IFluentEmail _fluentEmail;

    public EmailService(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public async Task SendEmailAsync(
        string to, 
        string subject, 
        string body,
        CancellationToken cancellationToken)
    {
        await _fluentEmail
            .To(to)
            .Subject(subject)
            .Body(body)
            .SendAsync(cancellationToken);
    }
}
