using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
using DomainRepo = MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Application.Services;

public class TipoPlatoService : ITipoPlatoService
{
    private readonly ITipoPlatoRepository _tipoPlatoRepository;

    public TipoPlatoService(ITipoPlatoRepository tipoPlatoRepository)
    {
        _tipoPlatoRepository = tipoPlatoRepository;
    }

    public async Task<TipoPlatoResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _tipoPlatoRepository.GetByIdAsync(id, ct);
        if (entity == null) return null;

        return MapToResponse(entity);
    }

    public async Task<IEnumerable<TipoPlatoResponse>> GetListAsync(string? filter, CancellationToken ct)
    {
        var entities = await _tipoPlatoRepository.GetListAsync(filter, ct);
        if (entities == null) return Enumerable.Empty<TipoPlatoResponse>();

        return entities.Select(MapToResponse);
    }

    public async Task<int> CreateAsync(TipoPlatoCreateRequest request, string currentUser, CancellationToken ct)
    {
        var entity = new DomainRepo.TipoPlatoInsertRequest
        {
            Tipplades = request.Tipplades,
            Usureg = currentUser
        };

        return await _tipoPlatoRepository.CreateAsync(entity, ct);
    }

    public async Task<bool> UpdateAsync(TipoPlatoUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var entity = new DomainRepo.TipoPlatoUpdateRequest
        {
            Id = request.Id,
            Tipplades = request.Tipplades,
            Codest = request.Codest,
            Usumod = currentUser
        };

        var rowsAffected = await _tipoPlatoRepository.UpdateAsync(entity, ct);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var rowsAffected = await _tipoPlatoRepository.DeleteAsync(id, ct);
        return rowsAffected > 0;
    }

    private static TipoPlatoResponse MapToResponse(TipoPlato entity)
    {
        return new TipoPlatoResponse
        {
            Id = entity.Id,
            Tipplades = entity.Tipplades,
            Codest = entity.Codest,
            Fecreg = entity.Fecreg,
            Usureg = entity.Usureg,
            Fecmod = entity.Fecmod,
            Usumod = entity.Usumod
        };
    }
}
