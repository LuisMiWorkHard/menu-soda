using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;

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

        return TipoPlatoResponse.FromEntity(entity);
    }
}
