using MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;
using MenuSoda.Application.Interfaces;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Infrastructure.Repositories;

public class MenuDiarioRepository : IMenuDiarioRepository
{
    private readonly GenericRepository _genericRepository;

    public MenuDiarioRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<int> CreateAsync(MenuDiarioCreateRequest request, string currentUser, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResponse>(
            "menusoda.sp_ins_menu_diario",
            new { p_mendiafec = request.Fecha, p_usureg = currentUser },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
        return result?.Id ?? 0;
    }

    public async Task<int> UpdateAsync(MenuDiarioUpdateRequest request, string currentUser, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResponse>(
            "menusoda.sp_upd_menu_diario",
            new { p_id = request.Id, p_codest = request.EstadoId, p_usumod = currentUser },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
        return result?.Id ?? 0;
    }

    public async Task<int> DeleteAsync(int id, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResponse>(
            "menusoda.sp_del_menu_diario",
            new { p_id = id },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
        return result?.Id ?? 0;
    }

    public async Task<MenuDiarioResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<MenuDiarioResponse>(
            "menusoda.sp_get_menu_diario_id",
            new { p_id = id },
            ct
        );
    }

    public async Task<IEnumerable<MenuDiarioListCustomResponse>> GetCustomListReportAsync(string? busqueda, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<MenuDiarioListCustomResponse>(
            "menusoda.sp_get_menu_diario_list_custom",
            new { p_busqueda = busqueda },
            ct
        ) ?? Enumerable.Empty<MenuDiarioListCustomResponse>();
    }

    public async Task<IEnumerable<MenuDiario>> GetListAsync(CancellationToken ct)
    {
        // Placeholder hasta tener el SP complejo
        return Enumerable.Empty<MenuDiario>();
    }
}
