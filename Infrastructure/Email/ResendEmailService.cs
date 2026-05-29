using Resend;
using Microsoft.Extensions.Options;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Options;

namespace MenuSoda.Infrastructure.Email;

public class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly ResendOptions _options;

    public ResendEmailService(IResend resend, IOptions<ResendOptions> opts)
    {
        _resend = resend;
        _options = opts.Value;
    }

    public async Task SendVerificationCodeAsync(string toEmail, string toName, string codigo, CancellationToken ct)
    {
        var message = new EmailMessage
        {
            From    = $"{_options.FromName} <{_options.FromEmail}>",
            Subject = "Tu código de verificación - MenuSoda",
            HtmlBody = $"""
                <p>Hola {toName},</p>
                <p>Tu código de verificación para recuperar tu contraseña es:</p>
                <h1 style="letter-spacing:8px;font-size:48px;font-weight:bold;color:#6750A4;text-align:center;">{codigo}</h1>
                <p>Este código expira en <strong>5 minutos</strong>. Si no solicitaste esto, ignora este mensaje.</p>
                """
        };
        message.To.Add(toEmail);

        await _resend.EmailSendAsync(message, ct);
    }
}
