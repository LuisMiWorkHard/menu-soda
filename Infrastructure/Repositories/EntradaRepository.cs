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

    public async Task<Entrada?> GetByIdAsync(EntradaGetByIdRequest request, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<Entrada>(
            "menusoda.sp_get_entrada_id",
            new { p_id = request.Id },
            ct
        );
    }

    public async Task<IEnumerable<Entrada>?> GetListAsync(EntradaGetListRequest request, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<Entrada>(
            "menusoda.sp_get_entrada_list",
            new { p_entdes = request.Entdes },
            ct
        );
    }

    public async Task<int> CreateAsync(EntradaInsertRequest request, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<dynamic>(
            "menusoda.sp_ins_entrada",
            new
            {
                p_entdes = request.Entdes,
                p_entdeslar = request.Entdeslar,
                p_codtipent = request.Codtipent,
                p_codima = request.Codima,
                p_usureg = request.Usureg
            },
            ct
        );
        return result?.id ?? 0;
    }

    public async Task<int> UpdateAsync(EntradaUpdateRequest request, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<dynamic>(
            "menusoda.sp_upd_entrada",
            new 
            { 
                p_id = request.Id,
                p_entdes = request.Entdes, 
                p_codtipent = request.Codtipent,
                p_codest = request.Codest,
                p_usumod = request.Usumod
            },
            ct
        );
        return result?.id ?? 0;
    }

    public async Task<int> DeleteAsync(int id, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<dynamic>(
            "menusoda.sp_del_entrada",
            new { p_id = id },
            ct
        );
        return result?.id ?? 0;
    }
}
