using MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;
using MenuSoda.Application.Interfaces;
using DomainRepo = MenuSoda.Application.Dto;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Infrastructure.Repositories;

public class TipoEntradaRepository : ITipoEntradaRepository
{
    private readonly GenericRepository _genericRepository;

    public TipoEntradaRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<TipoEntrada?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<TipoEntrada>(
            "menusoda.sp_get_tipo_entrada_id",
            new { p_id = id },
            ct
        );
    }

    public async Task<IEnumerable<TipoEntrada>?> GetListAsync(string? descripcion, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<TipoEntrada>(
            "menusoda.sp_get_tipo_entrada_list",
            new { p_tipentdes = descripcion },
            ct
        );
    }

    public async Task<int> CreateAsync(TipoEntradaCreateRequest request, string currentUser, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<DomainRepo.OperationIdResponse>(
            "menusoda.sp_ins_tipo_entrada",
            new
            {
                p_tipentdes = request.Descripcion,
                p_usureg = currentUser
            },
            ct
        );
        return result?.Id ?? 0;
    }

    public async Task<int> UpdateAsync(TipoEntradaUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<DomainRepo.OperationIdResponse>(
            "menusoda.sp_upd_tipo_entrada",
            new
            {
                p_id = request.Id,
                p_tipentdes = request.Descripcion,
                p_codest = request.EstadoId,
                p_usumod = currentUser
            },
            ct
        );
        return result?.Id ?? 0;
    }

    public async Task<int> DeleteAsync(int id, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<DomainRepo.OperationIdResponse>(
            "menusoda.sp_del_tipo_entrada",
            new { p_id = id },
            ct
        );
        return result?.Id ?? 0;
    }
}
