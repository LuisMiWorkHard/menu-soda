using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

public interface IMenuDiarioService
{
    Task<int> CreateAsync(MenuDiarioCreateServiceRequest request, CancellationToken ct);
    Task<bool> UpdateAsync(MenuDiarioUpdateServiceRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, string user, CancellationToken ct);
    Task<MenuDiarioResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<MenuDiarioListItemResponse>> GetListAsync(CancellationToken ct);
}
