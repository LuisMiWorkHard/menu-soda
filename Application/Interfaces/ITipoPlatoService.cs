using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

public interface ITipoPlatoService
{
    Task<TipoPlatoResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<TipoPlatoResponse>> GetListAsync(string? filter, CancellationToken ct);
    Task<int> CreateAsync(TipoPlatoCreateRequest request, string currentUser, CancellationToken ct);
    Task<bool> UpdateAsync(TipoPlatoUpdateRequest request, string currentUser, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
