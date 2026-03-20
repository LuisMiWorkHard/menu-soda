using MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Interfaces;

public interface IPlatoRepository
{
    Task<Plato?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<Plato>?> GetListAsync(string? nombre, CancellationToken ct);
    Task<int> CreateAsync(PlatoCreateRequest request, string currentUser, CancellationToken ct);
    Task<int> UpdateAsync(PlatoUpdateRequest request, string currentUser, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
