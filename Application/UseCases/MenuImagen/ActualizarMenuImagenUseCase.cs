using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;

namespace MenuSoda.Application.UseCases.MenuImagen;

public class ActualizarMenuImagenUseCase
{
    private readonly IMenuImagenRepository _repository;

    public ActualizarMenuImagenUseCase(IMenuImagenRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(MenuImagenUpdateRequest request, string currentUser, CancellationToken ct)
    {
        var rowsAffected = await _repository.UpdateAsync(request, currentUser, ct);
        return rowsAffected > 0;
    }
}
