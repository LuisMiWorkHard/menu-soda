using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Mappers;

namespace MenuSoda.Application.UseCases.TipoPlato;

public class ObtenerTipoPlatoPorIdUseCase
{
    private readonly ITipoPlatoRepository _repository;

    public ObtenerTipoPlatoPorIdUseCase(ITipoPlatoRepository repository)
    {
        _repository = repository;
    }

    public async Task<TipoPlatoResponse?> ExecuteAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null) return null;

        return TipoPlatoMapper.Map(entity);
    }
}
