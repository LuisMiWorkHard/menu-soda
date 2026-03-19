using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Infrastructure.Middleware;

namespace MenuSoda.Application.UseCases.Plato;

public class CrearPlatoUseCase
{
    private readonly IPlatoRepository _platoRepository;
    private readonly ITipoPlatoRepository _tipoPlatoRepository;

    public CrearPlatoUseCase(IPlatoRepository platoRepository, ITipoPlatoRepository tipoPlatoRepository)
    {
        _platoRepository = platoRepository;
        _tipoPlatoRepository = tipoPlatoRepository;
    }

    public async Task<int> ExecuteAsync(PlatoCreateRequest request, string currentUser, CancellationToken ct)
    {
        // Validar que el tipo de plato exista y esté activo
        var tipoPlato = await _tipoPlatoRepository.GetByIdAsync(request.TipoPlatoId, ct);
        if (tipoPlato == null || tipoPlato.Codest == 0)
        {
            throw new MenuSoda.Infrastructure.Middleware.GlobalExceptionHandler.CustomBusinessValidationException($"El tipo de plato seleccionado no existe o no está activo.");
        }

        return await _platoRepository.CreateAsync(request, currentUser, ct);
    }
}
