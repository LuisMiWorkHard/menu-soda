using MenuSoda.Application.Dto;
using MenuSoda.Application.Options;
using MenuSoda.Domain.Interfaces.Security;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Domain.Models.Security;
using MenuSoda.Domain.Models.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MenuSoda.Application.Interfaces;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshToken _refreshTokens;
    private readonly AuthOptions _authOptions;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher, 
        ITokenGenerator tokenGenerator, 
        IRefreshToken refreshTokens,
        IOptions<AuthOptions> authOptions,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _refreshTokens = refreshTokens;
        _authOptions = authOptions.Value;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginServiceRequest request, CancellationToken ct)
    {
        var user = await _userRepository.GetByDocumentAsync(new UsuarioGetByDocumentRequest
        {
            TipoDocumento = request.TipoDocumento,
            NumeroDocumento = request.NumeroDocumento
        }, ct);

        // 1. Verificar si el usuario existe
        if (user == null)
        {
            throw new UnauthorizedAccessException("Usuario ó contraseña inválidos.");
        }

        // 2. Verificar bloqueo de cuenta
        if (user.Usufecblo.HasValue && user.Usufecblo > DateTime.UtcNow)
        {
            var remaining = user.Usufecblo.Value - DateTime.UtcNow;
            throw new UnauthorizedAccessException($"La cuenta esta bloqueada por exceder los {_authOptions.MaxFailedAttempts} intentos permitidos. Intente de nuevo en {Math.Ceiling(remaining.TotalMinutes)} minutos.");
        }

        // 3. Verificar contraseña
        if (!_passwordHasher.Verify(request.Contrasena, user.Usuhash))
        {
            user.Usuintfall++;
            if (user.Usuintfall >= _authOptions.MaxFailedAttempts)
            {
                _logger.LogWarning("Cuenta bloqueada por múltiples intentos: User {UserId}", user.Id);
            }
            
            await _userRepository.ActualizarBloqueoAsync(user, _authOptions.MaxFailedAttempts, _authOptions.LockoutMinutes, ct);
            
            throw new UnauthorizedAccessException("Usuario ó contraseña inválidos.");
        }

        // 4. Login exitoso: Resetear contadores
        if (user.Usuintfall > 0 || user.Usufecblo.HasValue)
        {
            user.Usuintfall = 0;
            user.Usufecblo = null;
            await _userRepository.ActualizarBloqueoAsync(user, _authOptions.MaxFailedAttempts, _authOptions.LockoutMinutes, ct);
        }

        var accessToken = _tokenGenerator.GenerateToken(user);

        var refresh = await _refreshTokens.CreateAsync(new RefreshTokenCreateRequest
        {
            UserId = user.Id,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            DeviceId = request.DeviceId,
            GeoLat = request.Latitud,
            GeoLon = request.Longitud,
            DaysToExpire = _authOptions.RefreshTokenDaysToExpire
        }, ct);

        _logger.LogInformation(
            "Login exitoso: UserId={UserId}, IP={IpAddress}, DeviceId={DeviceId}",
            user.Id, 
            request.IpAddress, 
            request.DeviceId);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refresh.PlainText,
            RefreshTokenExpiresUtc = refresh.ExpiresUtc
        };
    }

    public async Task<RefreshServiceResponse> RefreshAsync(
        RefreshServiceRequest request,
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
        
        var rotated = await _refreshTokens.RotateAsync(new RefreshTokenRotateRequest
        {
            OldTokenId = tokenInfo.Id,
            UserId = tokenInfo.UserId,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            DeviceId = request.DeviceId,
            GeoLat = request.Latitud,
            GeoLon = request.Longitud,
            DaysToExpire = _authOptions.RefreshTokenDaysToExpire
        }, ct);

        var user = await _userRepository.GetByIdAsync(new UsuarioGetByIdRequest
        {
            Id = tokenInfo.UserId
        }, ct) ?? throw new UnauthorizedAccessException("Usuario no encontrado.");
        
        var accessToken = _tokenGenerator.GenerateToken(user);

        return new RefreshServiceResponse
        {
            AccessToken = accessToken,
            RefreshTokenPlainText = rotated.PlainText,
            RefreshTokenExpiresUtc = rotated.ExpiresUtc
        };
    }

    public async Task LogoutAsync(LogoutServiceRequest request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request.RefreshToken)) return;

        // Revocamos específicamente este token
        await _refreshTokens.RevokeAsync(request.RefreshToken, request.IpAddress, ct);
        _logger.LogInformation("Token revocado exitosamente via Logout.");
    }
}