using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;

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
        var list = await _platoRepository.GetListAsync(nombre, ct);
        return list?.Select(PlatoResponse.FromEntity) ?? Enumerable.Empty<PlatoResponse>();
    }
}
