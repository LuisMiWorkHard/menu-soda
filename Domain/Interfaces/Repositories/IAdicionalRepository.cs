using MenuSoda.Domain.Entities;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface IAdicionalRepository
{
    Task<Adicional?> GetByIdAsync(int id, CancellationToken ct);
}
