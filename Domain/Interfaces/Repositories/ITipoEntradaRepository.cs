using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface ITipoEntradaRepository
{
    Task<TipoEntrada?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<TipoEntrada>?> GetListAsync(string? descripcion, CancellationToken ct);
    Task<int> CreateAsync(TipoEntradaInsertRequest request, CancellationToken ct);
    Task<int> UpdateAsync(TipoEntradaUpdateRequest request, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
