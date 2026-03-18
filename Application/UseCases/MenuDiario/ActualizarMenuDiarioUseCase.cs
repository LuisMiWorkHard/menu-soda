using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Infrastructure.Middleware;
using MenuSoda.Infrastructure.Persistence;
using DomainRepo = MenuSoda.Domain.Models.Repositories;

namespace MenuSoda.Application.UseCases.MenuDiario;

public class ActualizarMenuDiarioUseCase
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

    public ActualizarMenuDiarioUseCase(
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

    public async Task<bool> ExecuteAsync(MenuDiarioUpdateRequest request, string currentUser, CancellationToken ct)
    {
        // 1. Validar existencia Header
        var currentMenu = await _menuDiarioRepository.GetByIdAsync(request.Id, ct);
        if (currentMenu == null || currentMenu.Codest == 0)
            throw new GlobalExceptionHandler.CustomBusinessValidationException($"El menú no existe.");

        // 2. Validaciones Hijos
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

        // 3. Transacción Explícita
        using var connection = _dapperContext.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // A. Update Header
            var headerUpdate = new DomainRepo.MenuDiarioUpdateRequest
            {
                Id = request.Id,
                Mendiafec = request.Mendiafec,
                Codest = request.Codest,
                Usumod = currentUser
            };
            await _menuDiarioRepository.UpdateAsync(headerUpdate, ct, transaction);

            // B. Gestión Hijos: Estrategia Delete-Reinsert

            // Imagen
            await _menuDiarioImagenRepository.DeleteByMenuLogicalAsync(new DomainRepo.MenuDiarioImagenDeleteRequest
            {
                Codmendia = request.Id,
                Usumod = currentUser
            }, ct, transaction);
            await _menuDiarioImagenRepository.AddAsync(new DomainRepo.MenuDiarioImagenInsertRequest
            {
                Codmendia = request.Id,
                Codima = request.ImagenId,
                Usureg = currentUser
            }, ct, transaction);

            // Entradas
            await _menuDiarioEntradaRepository.DeleteByMenuLogicalAsync(new DomainRepo.MenuDiarioEntradaDeleteRequest
            {
                Codmendia = request.Id,
                Usumod = currentUser
            }, ct, transaction);
            foreach (var entId in request.EntradasIds)
            {
                await _menuDiarioEntradaRepository.AddAsync(new DomainRepo.MenuDiarioEntradaInsertRequest
                {
                    Codmendia = request.Id,
                    Codent = entId,
                    Usureg = currentUser
                }, ct, transaction);
            }

            // Platos - Cascade Delete: Primero Adicionales, luego Platos
            await _menuDiarioPlatoAdicionalRepository.DeleteByMenuLogicalAsync(new DomainRepo.MenuDiarioPlatoAdicionalDeleteRequest
            {
                Codmendia = request.Id,
                Usumod = currentUser
            }, ct, transaction);

            await _menuDiarioPlatoRepository.DeleteByMenuLogicalAsync(new DomainRepo.MenuDiarioPlatoDeleteRequest
            {
                Codmendia = request.Id,
                Usumod = currentUser
            }, ct, transaction);

            // Re-insertar Platos + Adicionales
            foreach (var platoItem in request.Platos)
            {
                var idPlatoGenerado = await _menuDiarioPlatoRepository.AddAsync(new DomainRepo.MenuDiarioPlatoInsertRequest
                {
                    Codmendia = request.Id,
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
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
