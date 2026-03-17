using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Options;
using MenuSoda.Domain.Interfaces.Security;
using MenuSoda.Domain.Interfaces.Repositories;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MenuSoda.Application.UseCases.Auth;

public class ObtenerRefreshTokenUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshToken _refreshTokens;
    private readonly AuthOptions _authOptions;
    private readonly ILogger<ObtenerRefreshTokenUseCase> _logger;

    public ObtenerRefreshTokenUseCase(
        IUserRepository userRepository,
        ITokenGenerator tokenGenerator,
        IRefreshToken refreshTokens,
        IOptions<AuthOptions> authOptions,
        ILogger<ObtenerRefreshTokenUseCase> logger)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _refreshTokens = refreshTokens;
        _authOptions = authOptions.Value;
        _logger = logger;
    }

    public async Task<ObtenerRefreshTokenResponse> ExecuteAsync(
        ObtenerRefreshTokenRequest request,
        CancellationToken ct)
    {
        // Traer token (incluso si está revocado para detectar fraude)
        var tokenInfo = await _refreshTokens.GetByPlainAsync(request.RefreshToken, ct)
                        ?? throw new UnauthorizedAccessException("Token de refresco inexistente.");

        // 1. Verificar si el token ya fue revocado o usado (ReplacedByTokenId != null o RevokedAtUtc != null)
        if (tokenInfo.RevokedAtUtc.HasValue || tokenInfo.ReplacedByTokenId.HasValue)
        {
            // DETECCIÓN DE REUTILIZACIÓN: Posible robo de token.
            // Revocamos todas las sesiones del usuario por seguridad.
            await _refreshTokens.RevokeAllActiveAsync(tokenInfo.UserId, request.IpAddress, ct);
            _logger.LogCritical("Intento de reutilización de token detectado. Todas las sesiones revocadas para UserId={UserId}", tokenInfo.UserId);
            throw new UnauthorizedAccessException("Token de refresco inválido (reutilización detectada).");
        }

        // 2. Verificar expiración
        if (tokenInfo.ExpiresAtUtc < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Token de refresco expirado.");
        }

        // 3. Vincular a DeviceId (Binding)
        if (tokenInfo.DeviceId != request.DeviceId)
        {
            _logger.LogWarning("Intento de refresh desde dispositivo diferente. TokenDeviceId={Original}, RequestDeviceId={Current}", tokenInfo.DeviceId, request.DeviceId);
            throw new UnauthorizedAccessException("El token no pertenece a este dispositivo.");
        }

        var rotated = await _refreshTokens.RotateAsync(tokenInfo, request, _authOptions.RefreshTokenDaysToExpire, ct);

        var user = await _userRepository.GetByIdAsync(tokenInfo.UserId, ct)
                   ?? throw new UnauthorizedAccessException("Usuario no encontrado.");

        var accessToken = _tokenGenerator.GenerateToken(user);

        return new ObtenerRefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshTokenPlainText = rotated.PlainText,
            RefreshTokenExpiresUtc = rotated.ExpiresUtc
        };
    }
}
