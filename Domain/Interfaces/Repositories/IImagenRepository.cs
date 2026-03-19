using MenuSoda.Domain.Entities;
using MenuSoda.Application.Dto;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface IImagenRepository
{
    Task<Imagen?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<Imagen>?> GetListAsync(string? nombre, CancellationToken ct);
    Task<int> CreateAsync(ImagenCreateRequest request, string currentUser, CancellationToken ct);
    Task<int> UpdateAsync(ImagenUpdateRequest request, string currentUser, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
