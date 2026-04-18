using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Mappers;

namespace MenuSoda.Application.UseCases.Entrada;

public class ListarEntradasUseCase
{
    private readonly IEntradaRepository _entradaRepository;

    public ListarEntradasUseCase(IEntradaRepository entradaRepository)
    {
        _entradaRepository = entradaRepository;
    }

    public async Task<IEnumerable<EntradaResponse>> ExecuteAsync(string? filter, CancellationToken ct)
    {
        var list = await _entradaRepository.GetListAsync(filter, ct);
        return list?.Select(EntradaMapper.Map) ?? Enumerable.Empty<EntradaResponse>();
    }
}
