using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Infrastructure.Repositories;

public class MenuImagenRepository : IMenuImagenRepository
{
    private readonly GenericRepository _genericRepository;

    public MenuImagenRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<MenuImagen?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<MenuImagen>(
            "menusoda.sp_get_menu_imagen_id",
            new { p_id = id },
            ct
        );
    }

    public async Task<IEnumerable<MenuImagen>?> GetListAsync(CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<MenuImagen>(
            "menusoda.sp_get_menu_imagen_list",
            new { },
            ct
        );
    }

    public async Task<int> CreateAsync(MenuImagenCreateRequest request, string currentUser, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResponse>(
            "menusoda.sp_ins_menu_imagen",
            new
            {
                p_codima = request.ImagenId,
                p_usureg = currentUser
            },
            ct,
            "result_cur"
        );
        return result?.Id ?? 0;
    }

    public async Task<int> UpdateAsync(MenuImagenUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResponse>(
            "menusoda.sp_upd_menu_imagen",
            new
            {
                p_id = request.Id,
                p_codima = request.ImagenId,
                p_codest = request.EstadoId,
                p_usumod = currentUser
            },
            ct
        );
        return result?.Id ?? 0;
    }

    public async Task<int> DeleteAsync(int id, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResponse>(
            "menusoda.sp_del_menu_imagen",
            new { p_id = id },
            ct
        );
        return result?.Id ?? 0;
    }
}
