using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;

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

        return MapToResponse(entity);
    }

    private ImagenResponse MapToResponse(Domain.Entities.Imagen entity)
    {
        return new ImagenResponse
        {
            Id = entity.Id,
            Imarut = _storageService.GetSignedUrl(entity.Imarut),
            Imanom = entity.Imanom,
            Imaext = entity.Imaext,
            Codest = entity.Codest,
            Fecreg = entity.Fecreg,
            Usureg = entity.Usureg,
            Fecmod = entity.Fecmod,
            Usumod = entity.Usumod
        };
    }
}
