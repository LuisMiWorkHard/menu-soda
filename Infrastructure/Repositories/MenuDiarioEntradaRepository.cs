using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Dto;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Infrastructure.Repositories;

public class MenuDiarioEntradaRepository : IMenuDiarioEntradaRepository
{
    private readonly GenericRepository _genericRepository;

    public MenuDiarioEntradaRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task AddAsync(MenuDiarioEntradaInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        await _genericRepository.GetSingleByProcedureAsync<OperationIdResponse>(
            "menusoda.sp_ins_menu_diario_entrada",
            new { p_codmendia = request.Codmendia, p_codent = request.Codent, p_usureg = request.Usureg },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    public async Task DeleteByMenuLogicalAsync(MenuDiarioEntradaDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        await _genericRepository.ExecuteNonQueryProcedureAsync(
            "menusoda.sp_del_menu_diario_entrada_logic",
            new { p_codmendia = request.Codmendia, p_usumod = request.Usumod },
            ct,
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    public async Task<IEnumerable<MenuDiarioEntradaResponse>> GetByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<MenuDiarioEntradaResponse>(
            "menusoda.sp_list_menu_diario_entrada_by_menu",
            new { p_codmendia = request.Codmendia },
            ct
        ) ?? Enumerable.Empty<MenuDiarioEntradaResponse>();
    }
}
