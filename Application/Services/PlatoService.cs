using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
using DomainRepo = MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Application.Services;

public class PlatoService : IPlatoService
{
    private readonly IPlatoRepository _platoRepository;

    public PlatoService(IPlatoRepository platoRepository)
    {
        _platoRepository = platoRepository;
    }

    public async Task<PlatoResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _platoRepository.GetByIdAsync(id, ct);
        if (entity == null) return null;

        return MapToResponse(entity);
    }

    public async Task<IEnumerable<PlatoResponse>> GetListAsync(string? nombre, CancellationToken ct)
    {
        var entities = await _platoRepository.GetListAsync(nombre, ct);
        if (entities == null) return Enumerable.Empty<PlatoResponse>();

        return entities.Select(MapToResponse);
    }

    public async Task<int> CreateAsync(PlatoCreateRequest request, string currentUser, CancellationToken ct)
    {
        var entity = new DomainRepo.PlatoInsertRequest
        {
            Planom = request.Planom,
            Plades = request.Plades,
            Codtippla = request.Codtippla,
            Usureg = currentUser
        };

        return await _platoRepository.CreateAsync(entity, ct);
    }

    public async Task<bool> UpdateAsync(PlatoUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var entity = new DomainRepo.PlatoUpdateRequest
        {
            Id = request.Id,
            Planom = request.Planom,
            Plades = request.Plades,
            Codtippla = request.Codtippla,
            Codest = request.Codest,
            Usumod = currentUser
        };

        var rowsAffected = await _platoRepository.UpdateAsync(entity, ct);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var rowsAffected = await _platoRepository.DeleteAsync(id, ct);
        return rowsAffected > 0;
    }

    private static PlatoResponse MapToResponse(Plato entity)
    {
        return new PlatoResponse
        {
            Id = entity.Id,
            Planom = entity.Planom,
            Plades = entity.Plades,
            Codtippla = entity.Codtippla,
            Codest = entity.Codest,
            Fecreg = entity.Fecreg,
            Usureg = entity.Usureg,
            Fecmod = entity.Fecmod,
            Usumod = entity.Usumod
        };
    }
}
