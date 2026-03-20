using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;

namespace MenuSoda.Application.UseCases.TipoPlato;

public class CrearTipoPlatoUseCase
{
    private readonly ITipoPlatoRepository _repository;

    public CrearTipoPlatoUseCase(ITipoPlatoRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> ExecuteAsync(TipoPlatoCreateRequest request, string currentUser, CancellationToken ct)
    {
        return await _repository.CreateAsync(request, currentUser, ct);
    }
}
