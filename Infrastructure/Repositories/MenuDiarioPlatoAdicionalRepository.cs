using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Dto;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Infrastructure.Repositories;

public class MenuDiarioPlatoAdicionalRepository : IMenuDiarioPlatoAdicionalRepository
{
    private readonly GenericRepository _genericRepository;

    public MenuDiarioPlatoAdicionalRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task AddAsync(MenuDiarioPlatoAdicionalInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
        await _genericRepository.GetSingleByProcedureAsync<OperationIdResponse>(
            "menusoda.sp_ins_menu_diario_plato_adicional",
            new { p_codmendiapla = request.MenuDiarioPlatoId, p_codadi = request.AdicionalId, p_usureg = request.UsuarioRegistro },
            ct,
            "result_cur",
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    public async Task DeleteByMenuLogicalAsync(MenuDiarioPlatoAdicionalDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null)
    {
         await _genericRepository.ExecuteNonQueryProcedureAsync(
            "menusoda.sp_del_menu_diario_plato_adicional_logic_by_menu",
            new { p_codmendia = request.Codmendia, p_usumod = request.Usumod },
            ct,
            (Npgsql.NpgsqlTransaction?)transaction
        );
    }

    public async Task<IEnumerable<MenuDiarioPlatoAdicionalResponse>> GetByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<MenuDiarioPlatoAdicionalResponse>(
            "menusoda.sp_list_menu_diario_plato_adicional_by_menu",
            new { p_codmendia = request.MenuDiarioId },
            ct
        ) ?? Enumerable.Empty<MenuDiarioPlatoAdicionalResponse>();
    }
}
