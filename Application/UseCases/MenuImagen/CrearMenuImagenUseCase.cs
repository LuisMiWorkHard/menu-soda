using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;

namespace MenuSoda.Application.UseCases.MenuImagen;

public class CrearMenuImagenUseCase
{
    private readonly IMenuImagenRepository _repository;

    public CrearMenuImagenUseCase(IMenuImagenRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> ExecuteAsync(MenuImagenCreateRequest request, string currentUser, CancellationToken ct)
    {
        return await _repository.CreateAsync(request, currentUser, ct);
    }
}
