using MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Domain.Models.Repositories;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Infrastructure.Repositories;

public class PlatoRepository : IPlatoRepository
{
    private readonly GenericRepository _genericRepository;

    public PlatoRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<Plato?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<Plato>(
            "menusoda.sp_get_plato_id",
            new { p_id = id },
            ct
        );
    }

    public async Task<IEnumerable<Plato>?> GetListAsync(string? nombre, CancellationToken ct)
    {
        return await _genericRepository.GetListByProcedureAsync<Plato>(
            "menusoda.sp_get_plato_list",
            new { p_planom = nombre },
            ct
        );
    }

    public async Task<int> CreateAsync(PlatoCreateRequest request, string currentUser, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResult>(
            "menusoda.sp_ins_plato",
            new
            {
                p_planom = request.Planom,
                p_plades = request.Plades,
                p_codtippla = request.Codtippla,
                p_usureg = currentUser
            },
            ct
        );
        return result?.Id ?? 0;
    }

    public async Task<int> UpdateAsync(PlatoUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var result = await _genericRepository.GetSingleByProcedureAsync<OperationIdResult>(
            "menusoda.sp_upd_plato",
            new
            {
                p_id = request.Id,
                p_planom = request.Planom,
                p_plades = request.Plades,
                p_codtippla = request.Codtippla,
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
            "menusoda.sp_del_plato",
            new { p_id = id },
            ct
        );
        return result?.Id ?? 0;
    }
}
