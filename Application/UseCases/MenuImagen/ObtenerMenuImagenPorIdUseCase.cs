using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Mappers;

namespace MenuSoda.Application.UseCases.MenuImagen;

public class ObtenerMenuImagenPorIdUseCase
{
    private readonly IMenuImagenRepository _repository;

    public ObtenerMenuImagenPorIdUseCase(IMenuImagenRepository repository)
    {
        _repository = repository;
    }

    public async Task<MenuImagenResponse?> ExecuteAsync(int id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null) return null;
        return entity.ToResponse();
    }
}
