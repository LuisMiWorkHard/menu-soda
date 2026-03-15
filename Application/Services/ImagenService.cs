using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
using DomainRepo = MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Application.Services;

public class ImagenService : IImagenService
{
    private readonly IImagenRepository _imagenRepository;
    private readonly IStorageService _storageService;

    public ImagenService(IImagenRepository imagenRepository, IStorageService storageService)
    {
        _imagenRepository = imagenRepository;
        _storageService = storageService;
    }

    public async Task<ImagenResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _imagenRepository.GetByIdAsync(id, ct);
        if (entity == null) return null;

        return MapToResponse(entity);
    }

    public async Task<IEnumerable<ImagenResponse>> GetListAsync(string? nombre, CancellationToken ct)
    {
        var entities = await _imagenRepository.GetListAsync(nombre, ct);
        if (entities == null) return Enumerable.Empty<ImagenResponse>();

        return entities.Select(MapToResponse);
    }

    public async Task<int> CreateAsync(ImagenCreateServiceRequest request, CancellationToken ct)
    {
        var entity = new DomainRepo.ImagenInsertRequest
        {
            Imarut = request.Imarut,
            Imanom = request.Imanom,
            Imaext = request.Imaext,
            Usureg = request.Usureg
        };

        return await _imagenRepository.CreateAsync(entity, ct);
    }

    public async Task<bool> UpdateAsync(ImagenUpdateServiceRequest request, CancellationToken ct)
    {
        var entity = new DomainRepo.ImagenUpdateRequest
        {
            Id = request.Id,
            Imarut = request.Imarut,
            Imanom = request.Imanom,
            Imaext = request.Imaext,
            Codest = request.Codest,
            Usumod = request.Usumod
        };

        var rowsAffected = await _imagenRepository.UpdateAsync(entity, ct);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var rowsAffected = await _imagenRepository.DeleteAsync(id, ct);
        return rowsAffected > 0;
    }

    private ImagenResponse MapToResponse(Imagen entity)
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
