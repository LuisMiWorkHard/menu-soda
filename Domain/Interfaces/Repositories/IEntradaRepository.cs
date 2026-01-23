using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface IEntradaRepository
{
    Task<Entrada?> GetByIdAsync(EntradaGetByIdRequest request, CancellationToken ct);
    Task<IEnumerable<Entrada>?> GetListAsync(EntradaGetListRequest request, CancellationToken ct);
    Task<int> CreateAsync(EntradaInsertRequest request, CancellationToken ct);
    Task<int> UpdateAsync(EntradaUpdateRequest request, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
