using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.Imagen;

public class EliminarImagenUseCase
{
    private readonly IImagenRepository _imagenRepository;

    public EliminarImagenUseCase(IImagenRepository imagenRepository)
    {
        _imagenRepository = imagenRepository;
    }

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct)
    {
        var rowsAffected = await _imagenRepository.DeleteAsync(id, ct);
        return rowsAffected > 0;
    }
}
