using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Interfaces.Security;

namespace MenuSoda.Application.UseCases.Auth;

public class RestablecerContrasenaRecuperacionUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly GenericRepository _genericRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RestablecerContrasenaRecuperacionUseCase(
        IUserRepository userRepository,
        GenericRepository genericRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository    = userRepository;
        _genericRepository = genericRepository;
        _passwordHasher    = passwordHasher;
    }

    public async Task ExecuteAsync(int userId, RestablecerContrasenaRecuperacionRequest request, CancellationToken ct)
    {
        // 1. Verificar que el usuario verificó un código recientemente
        var authResult = await _genericRepository.GetSingleByProcedureAsync<AutorizadoResult>(
            "seguridad.sp_check_recuperacion_autorizada",
            new { p_usuario_id = userId },
            ct
        );

        if (authResult?.Autorizado != true)
            throw new UnauthorizedAccessException(
                "No tienes autorización para cambiar la contraseña. Verifica el código primero.");

        // 2. Validar coincidencia de contraseñas
        if (request.NuevaContrasena != request.ConfirmarContrasena)
            throw new ArgumentException("Las contraseñas no coinciden.");

        // 3. Actualizar hash
        var nuevoHash = _passwordHasher.Hash(request.NuevaContrasena);
        await _userRepository.ActualizarHashAsync(userId, nuevoHash, userId.ToString(), ct);
    }
}
