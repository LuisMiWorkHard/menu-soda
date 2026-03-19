using MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Domain.Models.Repositories;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Infrastructure.Repositories;

public class ImagenRepository : IImagenRepository
{
    private readonly GenericRepository _genericRepository;

    public ImagenRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<Imagen?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<Imagen>(
            "menusoda.sp_get_imagen_id",
            new { p_id = id },
            ct
        );
    }

    public async Task<IEnumerable<Imagen>?> GetListAsync(string? nombre, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<Imagen>(
            "menusoda.sp_get_imagen_list",
            new { p_imanom = nombre },
            ct
        );
    }

    public async Task<int> CreateAsync(ImagenCreateRequest request, string currentUser, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResult>(
            "menusoda.sp_ins_imagen",
            new
            {
                p_imarut = request.Ruta,
                p_imanom = request.Nombre,
                p_imaext = request.Extension,
                p_usureg = currentUser
            },
            ct
        );
        return result?.Id ?? 0;
    }

    public async Task<int> UpdateAsync(ImagenUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResult>(
            "menusoda.sp_upd_imagen",
            new
            {
                p_id = request.Id,
                p_imarut = request.Ruta,
                p_imanom = request.Nombre,
                p_imaext = request.Extension,
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
            "menusoda.sp_del_imagen",
            new { p_id = id },
            ct
        );
        return result?.Id ?? 0;
    }
}
