using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Domain.Models.Repositories;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Infrastructure.Repositories;

public class MenuDiarioImagenRepository : IMenuDiarioImagenRepository
{
    private readonly GenericRepository _genericRepository;

    public MenuDiarioImagenRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task AddAsync(MenuDiarioImagenInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        await _genericRepository.GetSingleByProcedureAsync<OperationIdResult>(
            "menusoda.sp_ins_menu_diario_imagen",
            new { p_codmendia = request.Codmendia, p_codima = request.Codima, p_usureg = request.Usureg },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    public async Task DeleteByMenuLogicalAsync(MenuDiarioImagenDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        await _genericRepository.ExecuteNonQueryProcedureAsync(
            "menusoda.sp_del_menu_diario_imagen_logic",
            new { p_codmendia = request.Codmendia, p_usumod = request.Usumod },
            ct,
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    public async Task<MenuDiarioImagenResponse?> GetByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<MenuDiarioImagenResponse>(
            "menusoda.sp_get_menu_diario_imagen_by_menu",
            new { p_codmendia = request.Codmendia },
            ct
        );
    }
}
