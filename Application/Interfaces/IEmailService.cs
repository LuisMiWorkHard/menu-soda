namespace MenuSoda.Application.Interfaces;

public interface IEmailService
{
    Task SendVerificationCodeAsync(string toEmail, string toName, string codigo, CancellationToken ct);
}
