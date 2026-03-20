using MenuSoda.Domain.Entities;
using MenuSoda.Application.Interfaces;

namespace MenuSoda.Infrastructure.Repositories;

public class AdicionalRepository : IAdicionalRepository
{
    private readonly GenericRepository _genericRepository;

    public AdicionalRepository(GenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<Adicional?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<Adicional>(
            "menusoda.sp_get_adicional_id",
            new { p_id = id },
            ct
        );
    }
}
