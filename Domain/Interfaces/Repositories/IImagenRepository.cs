using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface IImagenRepository
{
    Task<Imagen?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<Imagen>?> GetListAsync(string? nombre, CancellationToken ct);
    Task<int> CreateAsync(ImagenInsertRequest request, CancellationToken ct);
    Task<int> UpdateAsync(ImagenUpdateRequest request, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
}
