using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Domain.Models.Repositories;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Infrastructure.Repositories;

public class MenuDiarioPlatoRepository : IMenuDiarioPlatoRepository
{
    private readonly GenericRepository _genericRepository;

    public MenuDiarioPlatoRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<int> AddAsync(MenuDiarioPlatoInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResult>(
            "menusoda.sp_ins_menu_diario_plato",
            new { p_codmendia = request.Codmendia, p_codpla = request.Codpla, p_usureg = request.Usureg },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
        return result?.Id ?? 0;
    }

    public async Task DeleteByMenuLogicalAsync(MenuDiarioPlatoDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
         await _genericRepository.ExecuteNonQueryProcedureAsync(
            "menusoda.sp_del_menu_diario_plato_logic",
            new { p_codmendia = request.Codmendia, p_usumod = request.Usumod },
            ct,
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    public async Task<IEnumerable<MenuDiarioPlatoResponse>> GetByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<MenuDiarioPlatoResponse>(
            "menusoda.sp_list_menu_diario_plato_by_menu",
            new { p_codmendia = request.Codmendia },
            ct
        ) ?? Enumerable.Empty<MenuDiarioPlatoResponse>();
    }
}
