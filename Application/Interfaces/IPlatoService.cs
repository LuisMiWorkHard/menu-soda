using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

public interface IPlatoService
{
    Task<PlatoResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<PlatoResponse>> GetListAsync(string? nombre, CancellationToken ct);
    Task<int> CreateAsync(PlatoCreateServiceRequest request, CancellationToken ct);
    Task<bool> UpdateAsync(PlatoUpdateServiceRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
