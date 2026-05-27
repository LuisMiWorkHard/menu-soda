using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Mappers;

namespace MenuSoda.Application.UseCases.Perfil;

public class ObtenerPerfilUseCase
{
    private readonly IUserRepository _userRepository;

    public ObtenerPerfilUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PerfilResponse?> ExecuteAsync(int usuarioId, CancellationToken ct)
    {
        var usuario = await _userRepository.GetByIdAsync(usuarioId, ct);
        if (usuario == null) return null;

        return usuario.ToResponse();
    }
}
