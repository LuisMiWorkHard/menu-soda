using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
using DomainRepo = MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Application.Services;

public class TipoEntradaService : ITipoEntradaService
{
    private readonly ITipoEntradaRepository _tipoEntradaRepository;

    public TipoEntradaService(ITipoEntradaRepository tipoEntradaRepository)
    {
        _tipoEntradaRepository = tipoEntradaRepository;
    }

    public async Task<TipoEntradaResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _tipoEntradaRepository.GetByIdAsync(id, ct);
        if (entity == null) return null;

        return MapToResponse(entity);
    }

    public async Task<IEnumerable<TipoEntradaResponse>> GetListAsync(string? filter, CancellationToken ct)
    {
        var entities = await _tipoEntradaRepository.GetListAsync(filter, ct);
        if (entities == null) return Enumerable.Empty<TipoEntradaResponse>();

        return entities.Select(MapToResponse);
    }

    public async Task<int> CreateAsync(TipoEntradaCreateRequest request, string currentUser, CancellationToken ct)
    {
        var entity = new DomainRepo.TipoEntradaInsertRequest
        {
            Tipentdes = request.Tipentdes,
            Usureg = currentUser
        };

        return await _tipoEntradaRepository.CreateAsync(entity, ct);
    }

    public async Task<bool> UpdateAsync(TipoEntradaUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var entity = new DomainRepo.TipoEntradaUpdateRequest
        {
            Id = request.Id,
            Tipentdes = request.Tipentdes,
            Codest = request.Codest,
            Usumod = currentUser
        };

        var rowsAffected = await _tipoEntradaRepository.UpdateAsync(entity, ct);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var rowsAffected = await _tipoEntradaRepository.DeleteAsync(id, ct);
        return rowsAffected > 0;
    }

    private static TipoEntradaResponse MapToResponse(TipoEntrada entity)
    {
        return new TipoEntradaResponse
        {
            Id = entity.Id,
            Tipentdes = entity.Tipentdes,
            Codest = entity.Codest,
            Fecreg = entity.Fecreg,
            Usureg = entity.Usureg,
            Fecmod = entity.Fecmod,
            Usumod = entity.Usumod
        };
    }
}
