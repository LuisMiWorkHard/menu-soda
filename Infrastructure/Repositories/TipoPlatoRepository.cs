using MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Domain.Models.Repositories;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Infrastructure.Repositories;

public class TipoPlatoRepository : ITipoPlatoRepository
{
    private readonly GenericRepository _genericRepository;

    public TipoPlatoRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<TipoPlato?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<TipoPlato>(
            "menusoda.sp_get_tipo_plato_id",
            new { p_id = id },
            ct
        );
    }

    public async Task<IEnumerable<TipoPlato>?> GetListAsync(string? descripcion, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<TipoPlato>(
            "menusoda.sp_get_tipo_plato_list",
            new { p_tipplades = descripcion },
            ct
        );
    }

    public async Task<int> CreateAsync(TipoPlatoCreateRequest request, string currentUser, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResult>(
            "menusoda.sp_ins_tipo_plato",
            new
            {
                p_tipplades = request.Tipplades,
                p_usureg = currentUser
            },
            ct
        );
        return result?.Id ?? 0;
    }

    public async Task<int> UpdateAsync(TipoPlatoUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResult>(
            "menusoda.sp_upd_tipo_plato",
            new
            {
                p_id = request.Id,
                p_tipplades = request.Tipplades,
                p_codest = request.Codest,
                p_usumod = currentUser
            },
            ct
        );
        return result?.Id ?? 0;
    }

    public async Task<int> DeleteAsync(int id, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResult>(
            "menusoda.sp_del_tipo_plato",
            new { p_id = id },
            ct
        );
        return result?.Id ?? 0;
    }
}
