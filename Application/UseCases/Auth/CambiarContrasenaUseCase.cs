using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Interfaces.Security;
using MenuSoda.Application.Options;
using Microsoft.Extensions.Options;

namespace MenuSoda.Application.UseCases.Auth;

public class CambiarContrasenaUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly AuthOptions _authOptions;

    public CambiarContrasenaUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IOptions<AuthOptions> authOptions)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _authOptions = authOptions.Value;
    }

    public async Task ExecuteAsync(int usuarioId, CambiarContrasenaRequest request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(usuarioId, ct);
        if (user == null)
            throw new KeyNotFoundException("Usuario no encontrado.");

        // 1. Verificar bloqueo de cuenta
        if (user.Usufecblo.HasValue && user.Usufecblo > DateTime.UtcNow)
        {
            var remaining = user.Usufecblo.Value - DateTime.UtcNow;
            throw new UnauthorizedAccessException(
                $"La cuenta está bloqueada. Intente de nuevo en {Math.Ceiling(remaining.TotalMinutes)} minutos.");
        }

        // 2. Verificar contraseña actual
        if (!_passwordHasher.Verify(request.ContrasenaActual, user.Usuhash))
        {
            user.Usuintfall++;
            await _userRepository.ActualizarBloqueoAsync(
                user, _authOptions.MaxFailedAttempts, _authOptions.LockoutMinutes, ct);
            throw new UnauthorizedAccessException("Contraseña actual incorrecta.");
        }

        // 3. Login correcto: resetear contadores si es necesario
        if (user.Usuintfall > 0 || user.Usufecblo.HasValue)
        {
            user.Usuintfall = 0;
            user.Usufecblo = null;
            await _userRepository.ActualizarBloqueoAsync(
                user, _authOptions.MaxFailedAttempts, _authOptions.LockoutMinutes, ct);
        }

        // 4. Hashear la nueva contraseña y guardar
        var nuevoHash = _passwordHasher.Hash(request.ContrasenaNueva);
        await _userRepository.ActualizarHashAsync(user.Id, nuevoHash, "cambio_contrasena", ct);
    }
}
