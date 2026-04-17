using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Mappers;
using MenuSoda.Application.Utils;

namespace MenuSoda.Application.UseCases.Plato;

public class ListarPlatosUseCase
{
    private readonly IPlatoRepository _platoRepository;

    public ListarPlatosUseCase(IPlatoRepository platoRepository)
    {
        _platoRepository = platoRepository;
    }

    public async Task<IEnumerable<PlatoResponse>> ExecuteAsync(string? nombre, CancellationToken ct)
    {
        var list = await _platoRepository.GetListAsync(null, ct);
        var mapped = list?.Select(PlatoMapper.Map) ?? Enumerable.Empty<PlatoResponse>();
        if (string.IsNullOrWhiteSpace(nombre)) return mapped;
        return mapped.Where(p => FuzzyTextUtil.IsFuzzyMatch(nombre, p.Nombre));
    }
}
