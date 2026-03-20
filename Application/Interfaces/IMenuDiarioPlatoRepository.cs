using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

/// <summary>
/// Repositorio para gestionar la relación entre MenuDiario y Platos
/// </summary>
public interface IMenuDiarioPlatoRepository
{
    /// <summary>
    /// Retorna el ID del plato generado para poder insertar adicionales
    /// </summary>
    Task<int> AddAsync(MenuDiarioPlatoInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task DeleteByMenuLogicalAsync(MenuDiarioPlatoDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<IEnumerable<MenuDiarioPlatoResponse>> GetByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct);
}
