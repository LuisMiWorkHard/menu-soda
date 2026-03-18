using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.Plato;

public class EliminarPlatoUseCase
{
    private readonly IPlatoRepository _platoRepository;

    public EliminarPlatoUseCase(IPlatoRepository platoRepository)
    {
        _platoRepository = platoRepository;
    }

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct)
    {
        var rowsAffected = await _platoRepository.DeleteAsync(id, ct);
        return rowsAffected > 0;
    }
}
