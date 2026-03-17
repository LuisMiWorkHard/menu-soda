using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.Entrada;

public class ActualizarEntradaUseCase
{
    private readonly IEntradaRepository _entradaRepository;

    public ActualizarEntradaUseCase(IEntradaRepository entradaRepository)
    {
        _entradaRepository = entradaRepository;
    }

    public async Task<bool> ExecuteAsync(EntradaUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var id = await _entradaRepository.UpdateAsync(request, currentUser, ct);
        return id > 0;
    }
}
