using MenuSoda.Domain.Entities;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface IEntradaRepository
{
    Task<Entrada?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<Entrada>?> GetListAsync(string? filter, CancellationToken ct);
    Task<int> CreateAsync(Application.Dto.EntradaCreateRequest request, string currentUser, CancellationToken ct);
    Task<int> UpdateAsync(Application.Dto.EntradaUpdateRequest request, string currentUser, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
