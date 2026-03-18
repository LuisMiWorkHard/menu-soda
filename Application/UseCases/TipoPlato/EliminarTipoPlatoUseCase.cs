using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.TipoPlato;

public class EliminarTipoPlatoUseCase
{
    private readonly ITipoPlatoRepository _repository;

    public EliminarTipoPlatoUseCase(ITipoPlatoRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct)
    {
        var rowsAffected = await _repository.DeleteAsync(id, ct);
        return rowsAffected > 0;
    }
}
