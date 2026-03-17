using MenuSoda.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace MenuSoda.Application.UseCases.Auth;

public class LogoutUseCase
{
    private readonly IRefreshToken _refreshTokens;
    private readonly ILogger<LogoutUseCase> _logger;

    public LogoutUseCase(
        IRefreshToken refreshTokens,
        ILogger<LogoutUseCase> logger)
    {
        _refreshTokens = refreshTokens;
        _logger = logger;
    }

    public async Task ExecuteAsync(string? refreshToken, string? ipAddress, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(refreshToken)) return;

        await _refreshTokens.RevokeAsync(refreshToken, ipAddress, ct);
        _logger.LogInformation("Token revocado exitosamente via Logout.");
    }
}
