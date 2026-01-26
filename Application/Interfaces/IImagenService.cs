using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

public interface IImagenService
{
    Task<ImagenResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<ImagenResponse>> GetListAsync(string? nombre, CancellationToken ct);
    Task<int> CreateAsync(ImagenCreateServiceRequest request, CancellationToken ct);
    Task<bool> UpdateAsync(ImagenUpdateServiceRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
