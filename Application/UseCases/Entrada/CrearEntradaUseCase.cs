using MenuSoda.Domain.Exceptions;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;

namespace MenuSoda.Application.UseCases.Entrada;

public class CrearEntradaUseCase
{
    private readonly IEntradaRepository _entradaRepository;
    private readonly ITipoEntradaRepository _tipoEntradaRepository;

    public CrearEntradaUseCase(IEntradaRepository entradaRepository, ITipoEntradaRepository tipoEntradaRepository)
    {
        _entradaRepository = entradaRepository;
        _tipoEntradaRepository = tipoEntradaRepository;
    }

    public async Task<int> ExecuteAsync(EntradaCreateRequest request, string currentUser, CancellationToken ct)
    {
        var tipoEntrada = await _tipoEntradaRepository.GetByIdAsync(request.TipoEntradaId, ct);
        if (tipoEntrada == null || tipoEntrada.Codest == 0)
        {
            throw new CustomBusinessValidationException($"El tipo de entrada seleccionado no existe o no está activo.");
        }
        return await _entradaRepository.CreateAsync(request, currentUser, ct);
    }
}
