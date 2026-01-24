using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

public interface IPlatoService
{
    Task<PlatoResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<PlatoResponse>> GetListAsync(string? nombre, CancellationToken ct);
    Task<int> CreateAsync(PlatoCreateRequest request, string currentUser, CancellationToken ct);
    Task<bool> UpdateAsync(PlatoUpdateRequest request, string currentUser, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
