using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

public interface IEntradaService
{
    Task<EntradaResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<EntradaResponse>> GetListAsync(string? filter, CancellationToken ct);
    Task<int> CreateAsync(EntradaCreateRequest request, string currentUser, CancellationToken ct);
    Task<bool> UpdateAsync(EntradaUpdateRequest request, string currentUser, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
