using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Application.Services;

public class EntradaService : IEntradaService
{
    private readonly IEntradaRepository _entradaRepository;

    public EntradaService(IEntradaRepository entradaRepository)
    {
        _entradaRepository = entradaRepository;
    }

    public async Task<EntradaResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entrada = await _entradaRepository.GetByIdAsync(new EntradaGetByIdRequest { Id = id }, ct);
        if (entrada == null) return null;

        return MapToResponse(entrada);
    }

    public async Task<IEnumerable<EntradaResponse>> GetListAsync(string? filter, CancellationToken ct)
    {
        var list = await _entradaRepository.GetListAsync(new EntradaGetListRequest { Entdes = filter }, ct);
        return list?.Select(MapToResponse) ?? Enumerable.Empty<EntradaResponse>();
    }

    public async Task<int> CreateAsync(EntradaCreateRequest request, string currentUser, CancellationToken ct)
    {
        return await _entradaRepository.CreateAsync(new EntradaInsertRequest
        {
            Entdes = request.Entdes,
            Codtipent = request.Codtipent,
            Usureg = currentUser
        }, ct);
    }

    public async Task<bool> UpdateAsync(MenuSoda.Application.Dto.EntradaUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var id = await _entradaRepository.UpdateAsync(new Domain.Models.Repositories.EntradaUpdateRequest
        {
            Id = request.Id,
            Entdes = request.Entdes,
            Codtipent = request.Codtipent,
            Codest = request.Codest,
            Usumod = currentUser
        }, ct);
        return id > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var deletedId = await _entradaRepository.DeleteAsync(id, ct);
        return deletedId > 0;
    }

    private static EntradaResponse MapToResponse(Entrada entrada)
    {
        return new EntradaResponse
        {
            Id = entrada.Id,
            Entdes = entrada.Entdes,
            Codest = entrada.Codest,
            Codtipent = entrada.Codtipent,
            Fecreg = entrada.Fecreg,
            Usureg = entrada.Usureg,
            Fecmod = entrada.Fecmod,
            Usumod = entrada.Usumod
        };
    }
}
