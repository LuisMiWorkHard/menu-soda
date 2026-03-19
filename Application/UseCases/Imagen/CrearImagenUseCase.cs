using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;

namespace MenuSoda.Application.UseCases.Imagen;

public class CrearImagenUseCase
{
    private readonly IImagenRepository _imagenRepository;

    public CrearImagenUseCase(IImagenRepository imagenRepository)
    {
        _imagenRepository = imagenRepository;
    }

    public async Task<int> ExecuteAsync(ImagenCreateRequest request, string currentUser, CancellationToken ct)
    {
        return await _imagenRepository.CreateAsync(request, currentUser, ct);
    }
}
