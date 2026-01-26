using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

public interface ITipoPlatoService
{
    Task<TipoPlatoResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<TipoPlatoResponse>> GetListAsync(string? filter, CancellationToken ct);
    Task<int> CreateAsync(TipoPlatoCreateServiceRequest request, CancellationToken ct);
    Task<bool> UpdateAsync(TipoPlatoUpdateServiceRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
