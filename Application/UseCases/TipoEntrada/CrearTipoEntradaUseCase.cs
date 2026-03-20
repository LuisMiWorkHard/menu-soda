using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;

namespace MenuSoda.Application.UseCases.TipoEntrada;

public class CrearTipoEntradaUseCase
{
    private readonly ITipoEntradaRepository _repository;

    public CrearTipoEntradaUseCase(ITipoEntradaRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> ExecuteAsync(TipoEntradaCreateRequest request, string currentUser, CancellationToken ct)
    {
        return await _repository.CreateAsync(request, currentUser, ct);
    }
}
