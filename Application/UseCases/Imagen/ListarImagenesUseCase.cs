using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.Imagen;

public class ListarImagenesUseCase
{
    private readonly IImagenRepository _imagenRepository;
    private readonly IStorageService _storageService;

    public ListarImagenesUseCase(IImagenRepository imagenRepository, IStorageService storageService)
    {
        _imagenRepository = imagenRepository;
        _storageService = storageService;
    }

    public async Task<IEnumerable<ImagenResponse>> ExecuteAsync(string? nombre, CancellationToken ct)
    {
        var entities = await _imagenRepository.GetListAsync(nombre, ct);
        if (entities == null) return Enumerable.Empty<ImagenResponse>();

        return entities.Select(e => MenuSoda.Application.Mappers.ImagenMapper.ToResponse(e, _storageService));
    }


}
