using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Domain.Interfaces.Repositories;

/// <summary>
/// Repositorio para gestionar la entidad principal MenuDiario
/// </summary>
public interface IMenuDiarioRepository
{
    // CRUD básico de MenuDiario (solo entidad principal)
    Task<int> CreateAsync(MenuDiarioInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<int> UpdateAsync(MenuDiarioUpdateRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<int> DeleteAsync(int id, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<MenuDiarioResponse?> GetByIdAsync(int id, CancellationToken ct);

    // Listados
    Task<IEnumerable<MenuDiario>> GetListAsync(CancellationToken ct);
    Task<IEnumerable<MenuDiarioListCustomResponse>> GetCustomListReportAsync(CancellationToken ct);
}
