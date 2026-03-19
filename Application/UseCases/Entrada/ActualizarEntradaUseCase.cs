using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.Entrada;

public class ActualizarEntradaUseCase
{
    private readonly IEntradaRepository _entradaRepository;
    private readonly ITipoEntradaRepository _tipoEntradaRepository;

    public ActualizarEntradaUseCase(IEntradaRepository entradaRepository, ITipoEntradaRepository tipoEntradaRepository)
    {
        _entradaRepository = entradaRepository;
        _tipoEntradaRepository = tipoEntradaRepository;
    }

    public async Task<bool> ExecuteAsync(EntradaUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var tipoEntrada = await _tipoEntradaRepository.GetByIdAsync(request.TipoEntradaId, ct);
        if (tipoEntrada == null || tipoEntrada.Codest == 0)
        {
            throw new MenuSoda.Infrastructure.Middleware.GlobalExceptionHandler.CustomBusinessValidationException($"El tipo de entrada seleccionado no existe o no está activo.");
        }
        
        var id = await _entradaRepository.UpdateAsync(request, currentUser, ct);
        return id > 0;
    }
}
