using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Mappers;

namespace MenuSoda.Application.UseCases.MenuImagen;

public class ListarMenuImagenesUseCase
{
    private readonly IMenuImagenRepository _repository;
    private readonly IStorageService _storageService;

    public ListarMenuImagenesUseCase(IMenuImagenRepository repository, IStorageService storageService)
    {
        _repository = repository;
        _storageService = storageService;
    }

    public async Task<IEnumerable<MenuImagenResponse>> ExecuteAsync(CancellationToken ct)
    {
        var entities = await _repository.GetListAsync(ct);
        if (entities == null) return Enumerable.Empty<MenuImagenResponse>();
        return entities.Select(e => e.ToResponse(_storageService));
    }
}
