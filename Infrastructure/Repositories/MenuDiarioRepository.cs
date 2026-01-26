using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Domain.Models.Repositories;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Infrastructure.Repositories;

public class MenuDiarioRepository : IMenuDiarioRepository
{
    private readonly GenericRepository _genericRepository;

    public MenuDiarioRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    // Header Implementation
    public async Task<int> CreateHeaderAsync(MenuDiarioInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<dynamic>(
            "menusoda.sp_ins_menu_diario",
            new { p_mendiafec = request.Mendiafec, p_usureg = request.Usureg },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
        return result?.id ?? 0;
    }

    public async Task<int> UpdateHeaderAsync(MenuDiarioUpdateRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<dynamic>(
            "menusoda.sp_upd_menu_diario",
            new { p_id = request.Id, p_mendiafec = request.Mendiafec, p_codest = request.Codest, p_usumod = request.Usumod },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
        return result?.id ?? 0;
    }

    public async Task<int> DeleteHeaderAsync(int id, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<dynamic>(
            "menusoda.sp_del_menu_diario",
            new { p_id = id },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
        return result?.id ?? 0;
    }
    
    public async Task<dynamic?> GetHeaderByIdAsync(int id, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<dynamic>(
            "menusoda.sp_get_menu_diario_id",
            new { p_id = id },
            ct
        );
    }
    
    // Reporte
    public async Task<IEnumerable<dynamic>> GetCustomListReportAsync(CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<dynamic>(
            "menusoda.sp_get_menu_diario_list_custom",
            new { },
            ct
        ) ?? Enumerable.Empty<dynamic>();
    }

    public async Task<IEnumerable<MenuDiario>> GetListAsync(CancellationToken ct)
    {
        // Placeholder hasta tener el SP complejo
        return Enumerable.Empty<MenuDiario>(); 
    }

    // Details - Entradas
    public async Task AddEntradaAsync(MenuDiarioEntradaInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        await _genericRepository.GetSingleByProcedureAsync<dynamic>(
            "menusoda.sp_ins_menu_diario_entrada",
            new { p_codmendia = request.Codmendia, p_codent = request.Codent, p_usureg = request.Usureg },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    public async Task DeleteEntradasByMenuLogicalAsync(MenuDiarioEntradaDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        await _genericRepository.ExecuteNonQueryProcedureAsync(
            "menusoda.sp_del_menu_diario_entrada_logic",
            new { p_codmendia = request.Codmendia, p_usumod = request.Usumod },
            ct,
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    // Details - Platos

    // CORRECCIÓN: Implementaré el método actualizado
    public async Task<int> AddPlatoAsync(MenuDiarioPlatoInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<dynamic>(
            "menusoda.sp_ins_menu_diario_plato",
            new { p_codmendia = request.Codmendia, p_codpla = request.Codpla, p_usureg = request.Usureg },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
        return result?.id ?? 0;
    }

    public async Task DeletePlatosByMenuLogicalAsync(MenuDiarioPlatoDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
         await _genericRepository.ExecuteNonQueryProcedureAsync(
            "menusoda.sp_del_menu_diario_plato_logic",
            new { p_codmendia = request.Codmendia, p_usumod = request.Usumod },
            ct,
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    // Adicionales
    public async Task AddPlatoAdicionalAsync(MenuDiarioPlatoAdicionalInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        await _genericRepository.GetSingleByProcedureAsync<dynamic>(
            "menusoda.sp_ins_menu_diario_plato_adicional",
            new { p_codmendiapla = request.Codmendiapla, p_codadi = request.Codadi, p_usureg = request.Usureg },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    public async Task DeletePlatoAdicionalesByMenuLogicalAsync(MenuDiarioPlatoAdicionalDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
         await _genericRepository.ExecuteNonQueryProcedureAsync(
            "menusoda.sp_del_menu_diario_plato_adicional_logic_by_menu",
            new { p_codmendia = request.Codmendia, p_usumod = request.Usumod },
            ct,
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }
    
    public async Task<IEnumerable<dynamic>> GetPlatoAdicionalesByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<dynamic>(
            "menusoda.sp_list_menu_diario_plato_adicional_by_menu",
            new { p_codmendia = request.Codmendia },
            ct
        ) ?? Enumerable.Empty<dynamic>();
    }

    // Details - Imagen
    public async Task AddImagenAsync(MenuDiarioImagenInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        await _genericRepository.GetSingleByProcedureAsync<dynamic>(
            "menusoda.sp_ins_menu_diario_imagen",
            new { p_codmendia = request.Codmendia, p_codima = request.Codima, p_usureg = request.Usureg },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    public async Task DeleteImagenByMenuLogicalAsync(MenuDiarioImagenDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        await _genericRepository.ExecuteNonQueryProcedureAsync(
            "menusoda.sp_del_menu_diario_imagen_logic",
            new { p_codmendia = request.Codmendia, p_usumod = request.Usumod },
            ct,
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    // Get Details
    public async Task<IEnumerable<dynamic>> GetEntradasByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<dynamic>(
            "menusoda.sp_list_menu_diario_entrada_by_menu",
            new { p_codmendia = request.Codmendia },
            ct
        ) ?? Enumerable.Empty<dynamic>();
    }

    public async Task<IEnumerable<dynamic>> GetPlatosByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<dynamic>(
            "menusoda.sp_list_menu_diario_plato_by_menu",
            new { p_codmendia = request.Codmendia },
            ct
        ) ?? Enumerable.Empty<dynamic>();
    }
    
    public async Task<dynamic?> GetImagenByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<dynamic>(
            "menusoda.sp_get_menu_diario_imagen_by_menu",
            new { p_codmendia = request.Codmendia },
            ct
        );
    }
}
