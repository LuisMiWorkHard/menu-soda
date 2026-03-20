using MenuSoda.Domain.Entities;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

public interface ITipoEntradaRepository
{
    Task<TipoEntrada?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<TipoEntrada>?> GetListAsync(string? descripcion, CancellationToken ct);
    Task<int> CreateAsync(TipoEntradaCreateRequest request, string currentUser, CancellationToken ct);
    Task<int> UpdateAsync(TipoEntradaUpdateRequest request, string currentUser, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
