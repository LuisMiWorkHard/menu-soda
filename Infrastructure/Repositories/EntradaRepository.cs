using Dto = MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Domain.Models.Repositories;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Infrastructure.Repositories;

public class EntradaRepository : IEntradaRepository
{
    private readonly GenericRepository _genericRepository;

    public EntradaRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<Entrada?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<Entrada>(
            "menusoda.sp_get_entrada_id",
            new { p_id = id },
            ct
        );
    }

    public async Task<IEnumerable<Entrada>?> GetListAsync(string? filter, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<Entrada>(
            "menusoda.sp_get_entrada_list",
            new { p_entdes = filter },
            ct
        );
    }

    public async Task<int> CreateAsync(EntradaCreateRequest request, string currentUser, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResult>(
            "menusoda.sp_ins_entrada",
            new
            {
                p_entdes = request.Descripcion,
                p_entdeslar = request.DescripcionLarga,
                p_codtipent = request.TipoEntradaId,
                p_codima = request.ImagenId,
                p_usureg = currentUser
            },
            ct
        );
        return result?.Id ?? 0;
    }

    public async Task<int> UpdateAsync(EntradaUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResult>(
            "menusoda.sp_upd_entrada",
            new
            {
                p_id = request.Id,
                p_entdes = request.Descripcion,
                p_codtipent = request.TipoEntradaId,
                p_codest = request.EstadoId,
                p_usumod = currentUser
            },
            ct
        );
        return result?.Id ?? 0;
    }

    public async Task<int> DeleteAsync(int id, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResult>(
            "menusoda.sp_del_entrada",
            new { p_id = id },
            ct
        );
        return result?.Id ?? 0;
    }
}
