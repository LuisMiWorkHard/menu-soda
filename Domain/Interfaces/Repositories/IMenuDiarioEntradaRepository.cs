using MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Domain.Interfaces.Repositories;

/// <summary>
/// Repositorio para gestionar la relación entre MenuDiario y Entradas
/// </summary>
public interface IMenuDiarioEntradaRepository
{
    Task AddAsync(MenuDiarioEntradaInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task DeleteByMenuLogicalAsync(MenuDiarioEntradaDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<IEnumerable<MenuDiarioEntradaResponse>> GetByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct);
}
