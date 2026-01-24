using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface ITipoPlatoRepository
{
    Task<TipoPlato?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<TipoPlato>?> GetListAsync(string? descripcion, CancellationToken ct);
    Task<int> CreateAsync(TipoPlatoInsertRequest request, CancellationToken ct);
    Task<int> UpdateAsync(TipoPlatoUpdateRequest request, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
