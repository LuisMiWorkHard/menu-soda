using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

/// <summary>
/// Repositorio para gestionar los adicionales de los platos del menú diario
/// </summary>
public interface IMenuDiarioPlatoAdicionalRepository
{
    Task AddAsync(MenuDiarioPlatoAdicionalInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task DeleteByMenuLogicalAsync(MenuDiarioPlatoAdicionalDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<IEnumerable<MenuDiarioPlatoAdicionalResponse>> GetByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct);
}
