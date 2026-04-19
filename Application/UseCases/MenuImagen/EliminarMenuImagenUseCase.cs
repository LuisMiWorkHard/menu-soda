using MenuSoda.Application.Interfaces;

namespace MenuSoda.Application.UseCases.MenuImagen;

public class EliminarMenuImagenUseCase
{
    private readonly IMenuImagenRepository _repository;

    public EliminarMenuImagenUseCase(IMenuImagenRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct)
    {
        var rowsAffected = await _repository.DeleteAsync(id, ct);
        return rowsAffected > 0;
    }
}
