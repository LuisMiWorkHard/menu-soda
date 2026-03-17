using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.Entrada;

public class CrearEntradaUseCase
{
    private readonly IEntradaRepository _entradaRepository;

    public CrearEntradaUseCase(IEntradaRepository entradaRepository)
    {
        _entradaRepository = entradaRepository;
    }

    public async Task<int> ExecuteAsync(EntradaCreateRequest request, string currentUser, CancellationToken ct)
    {
        return await _entradaRepository.CreateAsync(request, currentUser, ct);
    }
}
