using MenuSoda.Application.Interfaces;

namespace MenuSoda.Application.UseCases.TipoEntrada;

public class EliminarTipoEntradaUseCase
{
    private readonly ITipoEntradaRepository _repository;

    public EliminarTipoEntradaUseCase(ITipoEntradaRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct)
    {
        var rowsAffected = await _repository.DeleteAsync(id, ct);
        return rowsAffected > 0;
    }
}
