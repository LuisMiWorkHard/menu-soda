using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.TipoEntrada;

public class ListarTiposEntradaUseCase
{
    private readonly ITipoEntradaRepository _repository;

    public ListarTiposEntradaUseCase(ITipoEntradaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TipoEntradaResponse>> ExecuteAsync(string? filter, CancellationToken ct)
    {
        var list = await _repository.GetListAsync(filter, ct);
        return list?.Select(TipoEntradaResponse.FromEntity) ?? Enumerable.Empty<TipoEntradaResponse>();
    }
}
