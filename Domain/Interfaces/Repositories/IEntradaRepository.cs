using MenuSoda.Domain.Entities;
using MenuSoda.Application.Dto;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface IEntradaRepository
{
    Task<Entrada?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<Entrada>?> GetListAsync(string? filter, CancellationToken ct);
    Task<int> CreateAsync(EntradaCreateRequest request, string currentUser, CancellationToken ct);
    Task<int> UpdateAsync(EntradaUpdateRequest request, string currentUser, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
