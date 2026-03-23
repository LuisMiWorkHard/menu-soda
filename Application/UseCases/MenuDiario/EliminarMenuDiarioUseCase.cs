using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Application.UseCases.MenuDiario;

public class EliminarMenuDiarioUseCase
{
    private readonly IMenuDiarioRepository _menuDiarioRepository;
    private readonly IMenuDiarioEntradaRepository _menuDiarioEntradaRepository;
    private readonly IMenuDiarioPlatoRepository _menuDiarioPlatoRepository;
    private readonly IMenuDiarioImagenRepository _menuDiarioImagenRepository;
    private readonly DapperContext _dapperContext;

    public EliminarMenuDiarioUseCase(
        IMenuDiarioRepository menuDiarioRepository,
        IMenuDiarioEntradaRepository menuDiarioEntradaRepository,
        IMenuDiarioPlatoRepository menuDiarioPlatoRepository,
        IMenuDiarioImagenRepository menuDiarioImagenRepository,
        DapperContext dapperContext)
    {
        _menuDiarioRepository = menuDiarioRepository;
        _menuDiarioEntradaRepository = menuDiarioEntradaRepository;
        _menuDiarioPlatoRepository = menuDiarioPlatoRepository;
        _menuDiarioImagenRepository = menuDiarioImagenRepository;
        _dapperContext = dapperContext;
    }

    public async Task<bool> ExecuteAsync(int id, string currentUser, CancellationToken ct)
    {
        using var connection = _dapperContext.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            await _menuDiarioRepository.DeleteAsync(id, ct, transaction);

            // Borrado lógico en cascada con Auditoría
            await _menuDiarioEntradaRepository.DeleteByMenuLogicalAsync(new MenuDiarioEntradaDeleteRequest
            {
                Codmendia = id,
                Usumod = currentUser
            }, ct, transaction);

            await _menuDiarioPlatoRepository.DeleteByMenuLogicalAsync(new MenuDiarioPlatoDeleteRequest
            {
                Codmendia = id,
                Usumod = currentUser
            }, ct, transaction);

            await _menuDiarioImagenRepository.DeleteByMenuLogicalAsync(new MenuDiarioImagenDeleteRequest
            {
                Codmendia = id,
                Usumod = currentUser
            }, ct, transaction);

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
