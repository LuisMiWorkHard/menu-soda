using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.TipoEntrada;

public class ObtenerTipoEntradaPorIdUseCase
{
    private readonly ITipoEntradaRepository _repository;

    public ObtenerTipoEntradaPorIdUseCase(ITipoEntradaRepository repository)
    {
        _repository = repository;
    }

    public async Task<TipoEntradaResponse?> ExecuteAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null) return null;

        return MenuSoda.Application.Mappers.AppMappers.Map(entity);
    }
}
