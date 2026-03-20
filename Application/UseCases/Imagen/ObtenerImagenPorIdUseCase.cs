using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;
using MenuSoda.Application.Mappers;

namespace MenuSoda.Application.UseCases.Imagen;

public class ObtenerImagenPorIdUseCase
{
    private readonly IImagenRepository _imagenRepository;
    private readonly IStorageService _storageService;

    public ObtenerImagenPorIdUseCase(IImagenRepository imagenRepository, IStorageService storageService)
    {
        _imagenRepository = imagenRepository;
        _storageService = storageService;
    }

    public async Task<ImagenResponse?> ExecuteAsync(int id, CancellationToken ct)
    {
        var entity = await _imagenRepository.GetByIdAsync(id, ct);
        if (entity == null) return null;

        return ImagenMapper.ToResponse(entity, _storageService);
    }


}
