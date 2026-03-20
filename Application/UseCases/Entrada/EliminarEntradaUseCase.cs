using MenuSoda.Application.Interfaces;

namespace MenuSoda.Application.UseCases.Entrada;

public class EliminarEntradaUseCase
{
    private readonly IEntradaRepository _entradaRepository;

    public EliminarEntradaUseCase(IEntradaRepository entradaRepository)
    {
        _entradaRepository = entradaRepository;
    }

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct)
    {
        var deletedId = await _entradaRepository.DeleteAsync(id, ct);
        return deletedId > 0;
    }
}
