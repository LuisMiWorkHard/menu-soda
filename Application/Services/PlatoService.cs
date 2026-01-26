using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
using DomainRepo = MenuSoda.Domain.Models.Repositories;

using MenuSoda.Infrastructure.Middleware; // Namespace del GlobalExceptionHandler

namespace MenuSoda.Application.Services;

public class PlatoService : IPlatoService
{
    private readonly IPlatoRepository _platoRepository;
    private readonly ITipoPlatoRepository _tipoPlatoRepository;

    public PlatoService(IPlatoRepository platoRepository, ITipoPlatoRepository tipoPlatoRepository)
    {
        _platoRepository = platoRepository;
        _tipoPlatoRepository = tipoPlatoRepository;
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

    public async Task<int> CreateAsync(PlatoCreateServiceRequest request, CancellationToken ct)
    {
        var tipoPlato = await _tipoPlatoRepository.GetByIdAsync(request.Codtippla, ct);
        if (tipoPlato == null || tipoPlato.Codest == 0)
        {
            throw new GlobalExceptionHandler.CustomBusinessValidationException($"El tipo de plato {request.Codtippla} no existe o no está activo.");
        }

        var entity = new DomainRepo.PlatoInsertRequest
        {
            Planom = request.Planom,
            Plades = request.Plades,
            Codtippla = request.Codtippla,
            Usureg = request.Usureg
        };

        return await _platoRepository.CreateAsync(entity, ct);
    }

    public async Task<bool> UpdateAsync(PlatoUpdateServiceRequest request, CancellationToken ct)
    {
        var tipoPlato = await _tipoPlatoRepository.GetByIdAsync(request.Codtippla, ct);
        if (tipoPlato == null || tipoPlato.Codest == 0)
        {
            throw new GlobalExceptionHandler.CustomBusinessValidationException($"El tipo de plato {request.Codtippla} no existe o no está activo.");
        }

        var entity = new DomainRepo.PlatoUpdateRequest
        {
            Id = request.Id,
            Planom = request.Planom,
            Plades = request.Plades,
            Codtippla = request.Codtippla,
            Codest = request.Codest,
            Usumod = request.Usumod
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
