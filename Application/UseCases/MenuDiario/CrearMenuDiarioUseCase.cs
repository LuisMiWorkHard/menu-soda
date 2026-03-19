using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Infrastructure.Middleware;
using MenuSoda.Infrastructure.Persistence;
using DomainRepo = MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Application.UseCases.MenuDiario;

public class CrearMenuDiarioUseCase
{
    private readonly IMenuDiarioRepository _menuDiarioRepository;
    private readonly IMenuDiarioEntradaRepository _menuDiarioEntradaRepository;
    private readonly IMenuDiarioPlatoRepository _menuDiarioPlatoRepository;
    private readonly IMenuDiarioPlatoAdicionalRepository _menuDiarioPlatoAdicionalRepository;
    private readonly IMenuDiarioImagenRepository _menuDiarioImagenRepository;
    private readonly IEntradaRepository _entradaRepository;
    private readonly IPlatoRepository _platoRepository;
    private readonly IImagenRepository _imagenRepository;
    private readonly IAdicionalRepository _adicionalRepository;
    private readonly DapperContext _dapperContext;

    public CrearMenuDiarioUseCase(
        IMenuDiarioRepository menuDiarioRepository,
        IMenuDiarioEntradaRepository menuDiarioEntradaRepository,
        IMenuDiarioPlatoRepository menuDiarioPlatoRepository,
        IMenuDiarioPlatoAdicionalRepository menuDiarioPlatoAdicionalRepository,
        IMenuDiarioImagenRepository menuDiarioImagenRepository,
        IEntradaRepository entradaRepository,
        IPlatoRepository platoRepository,
        IImagenRepository imagenRepository,
        IAdicionalRepository adicionalRepository,
        DapperContext dapperContext)
    {
        _menuDiarioRepository = menuDiarioRepository;
        _menuDiarioEntradaRepository = menuDiarioEntradaRepository;
        _menuDiarioPlatoRepository = menuDiarioPlatoRepository;
        _menuDiarioPlatoAdicionalRepository = menuDiarioPlatoAdicionalRepository;
        _menuDiarioImagenRepository = menuDiarioImagenRepository;
        _entradaRepository = entradaRepository;
        _platoRepository = platoRepository;
        _imagenRepository = imagenRepository;
        _adicionalRepository = adicionalRepository;
        _dapperContext = dapperContext;
    }

    public async Task<int> ExecuteAsync(MenuDiarioCreateRequest request, string currentUser, CancellationToken ct)
    {
        // 1. Validaciones previas
        var imagen = await _imagenRepository.GetByIdAsync(request.ImagenId, ct);
        if (imagen == null || imagen.Codest == 0)
            throw new GlobalExceptionHandler.CustomBusinessValidationException($"La imagen no existe o no está activa.");

        foreach (var entId in request.EntradasIds)
        {
            var ent = await _entradaRepository.GetByIdAsync(entId, ct);
            if (ent == null || ent.Codest == 0)
                throw new GlobalExceptionHandler.CustomBusinessValidationException($"La entrada no existe o no está activa.");
        }

        foreach (var platoItem in request.Platos)
        {
            var pla = await _platoRepository.GetByIdAsync(platoItem.PlatoId, ct);
            if (pla == null || pla.Codest == 0)
                throw new GlobalExceptionHandler.CustomBusinessValidationException($"El plato no existe o no está activo.");

            if (platoItem.AdicionalId.HasValue && platoItem.AdicionalId.Value > 0)
            {
                var adi = await _adicionalRepository.GetByIdAsync(platoItem.AdicionalId.Value, ct);
                if (adi == null || adi.Codest == 0)
                    throw new GlobalExceptionHandler.CustomBusinessValidationException($"El adicional no existe o no está activo.");
            }
        }

        // 2. Transacción Explícita
        using var connection = _dapperContext.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // A. Insertar Header
            var menuId = await _menuDiarioRepository.CreateAsync(request, currentUser, ct, transaction);

            // B. Insertar Detalles
            await _menuDiarioImagenRepository.AddAsync(new DomainRepo.MenuDiarioImagenInsertRequest
            {
                Codmendia = menuId,
                Codima = request.ImagenId,
                Usureg = currentUser
            }, ct, transaction);

            foreach (var entId in request.EntradasIds)
            {
                await _menuDiarioEntradaRepository.AddAsync(new DomainRepo.MenuDiarioEntradaInsertRequest
                {
                    Codmendia = menuId,
                    Codent = entId,
                    Usureg = currentUser
                }, ct, transaction);
            }

            foreach (var platoItem in request.Platos)
            {
                var idPlatoGenerado = await _menuDiarioPlatoRepository.AddAsync(new DomainRepo.MenuDiarioPlatoInsertRequest
                {
                    Codmendia = menuId,
                    Codpla = platoItem.PlatoId,
                    Usureg = currentUser
                }, ct, transaction);

                if (platoItem.AdicionalId.HasValue && platoItem.AdicionalId.Value > 0)
                {
                    await _menuDiarioPlatoAdicionalRepository.AddAsync(new DomainRepo.MenuDiarioPlatoAdicionalInsertRequest
                    {
                        Codmendiapla = idPlatoGenerado,
                        Codadi = platoItem.AdicionalId.Value,
                        Usureg = currentUser
                    }, ct, transaction);
                }
            }

            transaction.Commit();
            return menuId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
