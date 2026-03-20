using MenuSoda.Domain.Entities;
using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

/// <summary>
/// Repositorio para gestionar la entidad principal MenuDiario
/// </summary>
public interface IMenuDiarioRepository
{
    // CRUD básico de MenuDiario (solo entidad principal)
    Task<int> CreateAsync(MenuDiarioCreateRequest request, string currentUser, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<int> UpdateAsync(MenuDiarioUpdateRequest request, string currentUser, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<int> DeleteAsync(int id, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<MenuDiarioResponse?> GetByIdAsync(int id, CancellationToken ct);

    // Listados
    Task<IEnumerable<MenuDiario>> GetListAsync(CancellationToken ct);
    Task<IEnumerable<MenuDiarioListCustomResponse>> GetCustomListReportAsync(CancellationToken ct);
}
