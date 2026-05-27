using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Mappers;

namespace MenuSoda.Application.UseCases.Usuario;

public class ObtenerUsuarioPorIdUseCase
{
    private readonly IUserRepository _userRepository;

    public ObtenerUsuarioPorIdUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UsuarioResponse?> ExecuteAsync(int usuarioId, CancellationToken ct)
    {
        var usuario = await _userRepository.GetByIdAsync(usuarioId, ct);
        if (usuario == null) return null;

        return usuario.ToUsuarioResponse();
    }
}
