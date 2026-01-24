using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface IPlatoRepository
{
    Task<Plato?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<Plato>?> GetListAsync(string? nombre, CancellationToken ct);
    Task<int> CreateAsync(PlatoInsertRequest request, CancellationToken ct);
    Task<int> UpdateAsync(PlatoUpdateRequest request, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
