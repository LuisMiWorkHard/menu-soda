using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface IMenuDiarioRepository
{
    // Transacciones Header
    Task<int> CreateHeaderAsync(MenuDiarioInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<int> UpdateHeaderAsync(MenuDiarioUpdateRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<int> DeleteHeaderAsync(int id, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<dynamic?> GetHeaderByIdAsync(int id, CancellationToken ct);
    
    // Lista Header (Entidad)
    Task<IEnumerable<MenuDiario>> GetListAsync(CancellationToken ct); 

    // Reporte Personalizado
    Task<IEnumerable<dynamic>> GetCustomListReportAsync(CancellationToken ct);

    // Detalles (Usando Modelos Específicos)
    
    // Entradas
    Task AddEntradaAsync(MenuDiarioEntradaInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task DeleteEntradasByMenuLogicalAsync(MenuDiarioEntradaDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<IEnumerable<dynamic>> GetEntradasByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct);

    // Platos
    // Retorna ID para poder insertar adicionales
    Task<int> AddPlatoAsync(MenuDiarioPlatoInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null); 
    Task DeletePlatosByMenuLogicalAsync(MenuDiarioPlatoDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<IEnumerable<dynamic>> GetPlatosByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct);

    // Adicionales
    Task AddPlatoAdicionalAsync(MenuDiarioPlatoAdicionalInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task DeletePlatoAdicionalesByMenuLogicalAsync(MenuDiarioPlatoAdicionalDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<IEnumerable<dynamic>> GetPlatoAdicionalesByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct);

    // Imagen
    Task AddImagenAsync(MenuDiarioImagenInsertRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task DeleteImagenByMenuLogicalAsync(MenuDiarioImagenDeleteRequest request, CancellationToken ct, System.Data.IDbTransaction? transaction = null);
    Task<dynamic?> GetImagenByMenuAsync(MenuDiarioGetDetailByMenuRequest request, CancellationToken ct);
}
