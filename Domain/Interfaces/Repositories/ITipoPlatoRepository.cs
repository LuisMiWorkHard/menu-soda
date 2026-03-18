using MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface ITipoPlatoRepository
{
    Task<TipoPlato?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<TipoPlato>?> GetListAsync(string? descripcion, CancellationToken ct);
    Task<int> CreateAsync(TipoPlatoCreateRequest request, string currentUser, CancellationToken ct);
    Task<int> UpdateAsync(TipoPlatoUpdateRequest request, string currentUser, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
