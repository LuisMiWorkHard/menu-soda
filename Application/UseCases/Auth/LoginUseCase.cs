using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Options;
using MenuSoda.Domain.Interfaces.Security;
using MenuSoda.Domain.Interfaces.Repositories;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MenuSoda.Application.UseCases.Auth;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshToken _refreshTokens;
    private readonly AuthOptions _authOptions;
    private readonly ILogger<LoginUseCase> _logger;

    public LoginUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IRefreshToken refreshTokens,
        IOptions<AuthOptions> authOptions,
        ILogger<LoginUseCase> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _refreshTokens = refreshTokens;
        _authOptions = authOptions.Value;
        _logger = logger;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _userRepository.GetByDocumentAsync(request.TipoDocumento, request.NumeroDocumento, ct);

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

        var refresh = await _refreshTokens.CreateAsync(user, request, _authOptions.RefreshTokenDaysToExpire, ct);

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
}
