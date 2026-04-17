using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Mappers;
using MenuSoda.Application.Utils;

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
        var list = await _entradaRepository.GetListAsync(null, ct);
        var mapped = list?.Select(EntradaMapper.Map) ?? Enumerable.Empty<EntradaResponse>();
        if (string.IsNullOrWhiteSpace(filter)) return mapped;
        return mapped.Where(e => FuzzyTextUtil.IsFuzzyMatch(filter, e.Nombre));
    }
}
