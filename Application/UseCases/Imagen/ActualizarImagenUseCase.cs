using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.Imagen;

public class ActualizarImagenUseCase
{
    private readonly IImagenRepository _imagenRepository;

    public ActualizarImagenUseCase(IImagenRepository imagenRepository)
    {
        _imagenRepository = imagenRepository;
    }

    public async Task<bool> ExecuteAsync(ImagenUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var rowsAffected = await _imagenRepository.UpdateAsync(request, currentUser, ct);
        return rowsAffected > 0;
    }
}
