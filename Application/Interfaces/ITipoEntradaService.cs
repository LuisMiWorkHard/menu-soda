using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

public interface ITipoEntradaService
{
    Task<TipoEntradaResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<TipoEntradaResponse>> GetListAsync(string? filter, CancellationToken ct);
    Task<int> CreateAsync(TipoEntradaCreateServiceRequest request, CancellationToken ct);
    Task<bool> UpdateAsync(TipoEntradaUpdateServiceRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
