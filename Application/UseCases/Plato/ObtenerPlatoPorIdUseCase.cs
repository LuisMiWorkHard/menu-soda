using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Mappers;

namespace MenuSoda.Application.UseCases.Plato;

public class ObtenerPlatoPorIdUseCase
{
    private readonly IPlatoRepository _platoRepository;

    public ObtenerPlatoPorIdUseCase(IPlatoRepository platoRepository)
    {
        _platoRepository = platoRepository;
    }

    public async Task<PlatoResponse?> ExecuteAsync(int id, CancellationToken ct)
    {
        var plato = await _platoRepository.GetByIdAsync(id, ct);
        if (plato == null) return null;

        return PlatoMapper.Map(plato);
    }
}
