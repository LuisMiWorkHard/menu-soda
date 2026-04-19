using MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Interfaces;

public interface IMenuImagenRepository
{
    Task<MenuImagen?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<MenuImagen>?> GetListAsync(CancellationToken ct);
    Task<int> CreateAsync(MenuImagenCreateRequest request, string currentUser, CancellationToken ct);
    Task<int> UpdateAsync(MenuImagenUpdateRequest request, string currentUser, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
