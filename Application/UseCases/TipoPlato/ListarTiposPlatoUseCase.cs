using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.TipoPlato;

public class ListarTiposPlatoUseCase
{
    private readonly ITipoPlatoRepository _repository;

    public ListarTiposPlatoUseCase(ITipoPlatoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TipoPlatoResponse>> ExecuteAsync(string? filter, CancellationToken ct)
    {
        var list = await _repository.GetListAsync(filter, ct);
        return list?.Select(MenuSoda.Application.Mappers.AppMappers.Map) ?? Enumerable.Empty<TipoPlatoResponse>();
    }
}
