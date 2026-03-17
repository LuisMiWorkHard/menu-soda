using MenuSoda.Domain.Entities;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface ITipoEntradaRepository
{
    Task<TipoEntrada?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<TipoEntrada>?> GetListAsync(string? descripcion, CancellationToken ct);
    Task<int> CreateAsync(Application.Dto.TipoEntradaCreateRequest request, string currentUser, CancellationToken ct);
    Task<int> UpdateAsync(Application.Dto.TipoEntradaUpdateRequest request, string currentUser, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
