using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.TipoPlato;

public class ActualizarTipoPlatoUseCase
{
    private readonly ITipoPlatoRepository _repository;

    public ActualizarTipoPlatoUseCase(ITipoPlatoRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(TipoPlatoUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var id = await _repository.UpdateAsync(request, currentUser, ct);
        return id > 0;
    }
}
