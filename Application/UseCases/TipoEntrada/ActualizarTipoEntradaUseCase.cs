using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;

namespace MenuSoda.Application.UseCases.TipoEntrada;

public class ActualizarTipoEntradaUseCase
{
    private readonly ITipoEntradaRepository _repository;

    public ActualizarTipoEntradaUseCase(ITipoEntradaRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(TipoEntradaUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var id = await _repository.UpdateAsync(request, currentUser, ct);
        return id > 0;
    }
}
