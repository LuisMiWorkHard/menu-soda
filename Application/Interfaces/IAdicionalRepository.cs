using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Interfaces;

public interface IAdicionalRepository
{
    Task<Adicional?> GetByIdAsync(int id, CancellationToken ct);
}
