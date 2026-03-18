using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Infrastructure.Middleware;

namespace MenuSoda.Application.UseCases.Plato;

public class ActualizarPlatoUseCase
{
    private readonly IPlatoRepository _platoRepository;
    private readonly ITipoPlatoRepository _tipoPlatoRepository;

    public ActualizarPlatoUseCase(IPlatoRepository platoRepository, ITipoPlatoRepository tipoPlatoRepository)
    {
        _platoRepository = platoRepository;
        _tipoPlatoRepository = tipoPlatoRepository;
    }

    public async Task<bool> ExecuteAsync(PlatoUpdateRequest request, string currentUser, CancellationToken ct)
    {
        // Validar que el tipo de plato exista y esté activo
        var tipoPlato = await _tipoPlatoRepository.GetByIdAsync(request.Codtippla, ct);
        if (tipoPlato == null || tipoPlato.Codest == 0)
        {
            throw new GlobalExceptionHandler.CustomBusinessValidationException($"El tipo de plato {request.Codtippla} no existe o no está activo.");
        }

        var rowsAffected = await _platoRepository.UpdateAsync(request, currentUser, ct);
        return rowsAffected > 0;
    }
}
