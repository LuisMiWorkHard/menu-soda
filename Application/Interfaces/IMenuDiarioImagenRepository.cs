using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

/// <summary>
/// Repositorio para gestionar la imagen asociada al menú diario
/// </summary>
public interface IMenuDiarioImagenRepository
{
    Task AddAsync(MenuDiarioImagenInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task DeleteByMenuLogicalAsync(MenuDiarioImagenDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<MenuDiarioImagenResponse?> GetByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct);
}
