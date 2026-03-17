using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.Entrada;

public class ObtenerEntradaPorIdUseCase
{
    private readonly IEntradaRepository _entradaRepository;

    public ObtenerEntradaPorIdUseCase(IEntradaRepository entradaRepository)
    {
        _entradaRepository = entradaRepository;
    }

    public async Task<EntradaResponse?> ExecuteAsync(int id, CancellationToken ct)
    {
        var entrada = await _entradaRepository.GetByIdAsync(id, ct);
        if (entrada == null) return null;

        return EntradaResponse.FromEntity(entrada);
    }
}
