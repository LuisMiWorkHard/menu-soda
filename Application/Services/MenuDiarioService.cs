using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
using MenuSoda.Infrastructure.Middleware; // For Exceptions
using DomainRepo = MenuSoda.Domain.Models.Repositories;
using System.Transactions; // For TransactionScope

using MenuSoda.Infrastructure.Persistence;

namespace MenuSoda.Application.Services;

public class MenuDiarioService : IMenuDiarioService
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
    private readonly DapperContext _dapperContext; // Unit of Work manual

    public MenuDiarioService(
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

    public async Task<int> CreateAsync(MenuDiarioCreateServiceRequest request, CancellationToken ct)
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
            var headerRequest = new DomainRepo.MenuDiarioInsertRequest
            {
                Mendiafec = request.Mendiafec,
                Usureg = request.Usureg
            };
            var menuId = await _menuDiarioRepository.CreateAsync(headerRequest, ct, transaction);

            // B. Insertar Detalles
            await _menuDiarioImagenRepository.AddAsync(new DomainRepo.MenuDiarioImagenInsertRequest 
            { 
                Codmendia = menuId, 
                Codima = request.ImagenId, 
                Usureg = request.Usureg 
            }, ct, transaction);

            foreach (var entId in request.EntradasIds)
            {
                await _menuDiarioEntradaRepository.AddAsync(new DomainRepo.MenuDiarioEntradaInsertRequest
                {
                    Codmendia = menuId,
                    Codent = entId,
                    Usureg = request.Usureg
                }, ct, transaction);
            }

            foreach (var platoItem in request.Platos)
            {
                var idPlatoGenerado = await _menuDiarioPlatoRepository.AddAsync(new DomainRepo.MenuDiarioPlatoInsertRequest
                {
                    Codmendia = menuId,
                    Codpla = platoItem.PlatoId,
                    Usureg = request.Usureg
                }, ct, transaction);

                if (platoItem.AdicionalId.HasValue && platoItem.AdicionalId.Value > 0)
                {
                    await _menuDiarioPlatoAdicionalRepository.AddAsync(new DomainRepo.MenuDiarioPlatoAdicionalInsertRequest
                    {
                        Codmendiapla = idPlatoGenerado,
                        Codadi = platoItem.AdicionalId.Value,
                        Usureg = request.Usureg
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

    public async Task<bool> UpdateAsync(MenuDiarioUpdateServiceRequest request, CancellationToken ct)
    {
         // 1. Validar existencia Header
         var currentMenu = await _menuDiarioRepository.GetByIdAsync(request.Id, ct);
         if (currentMenu == null || currentMenu.Codest == 0)
             throw new GlobalExceptionHandler.CustomBusinessValidationException($"El menú no existe.");

        // 2. Validaciones Hijos (Igual que create)
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
                Usumod = request.Usumod
            };
            await _menuDiarioRepository.UpdateAsync(headerUpdate, ct, transaction);

            // B. Gestión Hijos: Estrategia simple -> Borrado Lógico + Re-inserción
            
            // Imagen
            await _menuDiarioImagenRepository.DeleteByMenuLogicalAsync(new DomainRepo.MenuDiarioImagenDeleteRequest 
            { 
                Codmendia = request.Id, 
                Usumod = request.Usumod 
            }, ct, transaction);
            await _menuDiarioImagenRepository.AddAsync(new DomainRepo.MenuDiarioImagenInsertRequest
            {
                Codmendia = request.Id,
                Codima = request.ImagenId,
                Usureg = request.Usumod
            }, ct, transaction);

            // Entradas
            await _menuDiarioEntradaRepository.DeleteByMenuLogicalAsync(new DomainRepo.MenuDiarioEntradaDeleteRequest
            {
                Codmendia = request.Id,
                Usumod = request.Usumod
            }, ct, transaction);
            foreach (var entId in request.EntradasIds)
            {
                await _menuDiarioEntradaRepository.AddAsync(new DomainRepo.MenuDiarioEntradaInsertRequest
                {
                    Codmendia = request.Id,
                    Codent = entId,
                    Usureg = request.Usumod
                }, ct, transaction);
            }
            
            // Platos - Cascade Delete: Primero Adicionales, luego Platos
            await _menuDiarioPlatoAdicionalRepository.DeleteByMenuLogicalAsync(new DomainRepo.MenuDiarioPlatoAdicionalDeleteRequest
            {
                Codmendia = request.Id,
                Usumod = request.Usumod
            }, ct, transaction);

            await _menuDiarioPlatoRepository.DeleteByMenuLogicalAsync(new DomainRepo.MenuDiarioPlatoDeleteRequest
            {
                Codmendia = request.Id,
                Usumod = request.Usumod
            }, ct, transaction);

            // Re-insertar Platos + Adicionales
            foreach (var platoItem in request.Platos)
            {
                var idPlatoGenerado = await _menuDiarioPlatoRepository.AddAsync(new DomainRepo.MenuDiarioPlatoInsertRequest
                {
                    Codmendia = request.Id,
                    Codpla = platoItem.PlatoId,
                    Usureg = request.Usumod
                }, ct, transaction);

                if (platoItem.AdicionalId.HasValue && platoItem.AdicionalId.Value > 0)
                {
                    await _menuDiarioPlatoAdicionalRepository.AddAsync(new DomainRepo.MenuDiarioPlatoAdicionalInsertRequest
                    {
                        Codmendiapla = idPlatoGenerado,
                        Codadi = platoItem.AdicionalId.Value,
                        Usureg = request.Usumod
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

    public async Task<bool> DeleteAsync(int id, string user, CancellationToken ct)
    {
         using var connection = _dapperContext.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            await _menuDiarioRepository.DeleteAsync(id, ct, transaction);
            
            // Borrado lógico en cascada con Auditoría
            await _menuDiarioEntradaRepository.DeleteByMenuLogicalAsync(new DomainRepo.MenuDiarioEntradaDeleteRequest 
            {
                Codmendia = id,
                Usumod = user
            }, ct, transaction);

            await _menuDiarioPlatoRepository.DeleteByMenuLogicalAsync(new DomainRepo.MenuDiarioPlatoDeleteRequest
            {
                Codmendia = id,
                Usumod = user
            }, ct, transaction);

            await _menuDiarioImagenRepository.DeleteByMenuLogicalAsync(new DomainRepo.MenuDiarioImagenDeleteRequest
            {
                Codmendia = id,
                Usumod = user
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

    public async Task<MenuDiarioDetailResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        var header = await _menuDiarioRepository.GetByIdAsync(id, ct);
        if (header == null) return null;

        var requestDetail = new DomainRepo.MenuDiarioGetDetailByMenuRequest { Codmendia = id };

        var entradas = await _menuDiarioEntradaRepository.GetByMenuAsync(requestDetail, ct);
        var platos = await _menuDiarioPlatoRepository.GetByMenuAsync(requestDetail, ct);
        var adicionales = await _menuDiarioPlatoAdicionalRepository.GetByMenuAsync(requestDetail, ct);
        var imagen = await _menuDiarioImagenRepository.GetByMenuAsync(requestDetail, ct);

        // Map Platos + Adicionals
        var platosList = new List<MenuDiarioPlatoWithAdicionalResponse>();
        if (platos != null)
        {
            foreach (var p in platos)
            {
                // Buscamos si tiene adicional. El SP de adicionales devuelve codmendiapla
                // p.Id es el ID de menu_diario_plato
                var adicional = adicionales?.FirstOrDefault(a => a.Codmendiapla == p.Id);

                platosList.Add(new MenuDiarioPlatoWithAdicionalResponse
                {
                    Id = p.Id,
                    Codpla = p.Codpla,
                    Planom = p.Planom,
                    Plades = p.Plades,
                    Codtippla = p.Codtippla,
                    Tipplades = p.Tipplades,
                    Codest = p.Codest,
                    Adicional = adicional
                });
            }
        }

        return new MenuDiarioDetailResponse
        {
            Id = header.Id,
            Mendiafec = header.Mendiafec, // String from DB
            Codest = header.Codest,
            Fecreg = header.Fecreg, // String from DB
            Usureg = header.Usureg,
            Fecmod = header.Fecmod, // String from DB (nullable)
            Usumod = header.Usumod,
            Entradas = entradas.ToList(),
            Platos = platosList,
            Imagen = imagen
        };
    }

    public async Task<IEnumerable<MenuDiarioListItemResponse>> GetListAsync(CancellationToken ct)
    {
        var rawList = await _menuDiarioRepository.GetListAsync(ct);
        
        // El repositorio devolverá dynamic o un modelo intermedio. Asumiremos dynamic por flexibilidad con Dapper
        // y mapearemos aquí usando los helpers.
        
        // NOTA: Para que esto funcione, IMenuDiarioRepository.GetListAsync debe llamar a sp_get_menu_diario_list_custom
        // y devolver una estructura que podamos leer.
        
        /* 
           Como IMenuDiarioRepository.GetListAsync devuelve IEnumerable<MenuDiario> (entidad pura),
           necesitamos cambiarlo o extenderlo. Por ahora, asumiré que el repositorio devuelve la data cruda necesaria
           o que hago el mapeo extra aquí si el repo lo soporta.
           
           Dado que definimos `Task<IEnumerable<MenuDiario>> GetListAsync` en la interfaz, 
           estrictamente hablando solo devuelve la entidad.
           
           VOY A USAR _genericRepository directamente AQUÍ o CASTEAR si es necesario, 
           pero lo correcto es que el repositorio tenga un método específico `GetUnamappedListAsync` o similar.
           
           Para no romper la interfaz existente, voy a asumir que GetListAsync en el repo
           está implementado para llamar al SP y devolver dynamic o un DTO de capa de infra.
           
           Vamos a modificar el Repositorio primero para que GetListAsync devuelva lo correcto (dynamic) 
           o crear un método nuevo en la interfaz. 
           
           Mejor opción ahora: Usar un método nuevo en el repositorio `GetCustomReportAsync`.
        */

        var reportData = await _menuDiarioRepository.GetCustomListReportAsync(ct);

        var response = new List<MenuDiarioListItemResponse>();

        foreach (var item in reportData)
        {
            string fechaStr = item.Mendiafec;
            string? fecModStr = item.Fecmod ?? item.Fecreg; 
            
            DateTime fechaVal;
            DateTime.TryParseExact(fechaStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fechaVal);

            DateTime? fecModVal = null;
            if (!string.IsNullOrEmpty(fecModStr))
            {
                 DateTime tmp;
                 // Formato con hora HH:mm en SP? Revisar SP.
                 // SP usa 'DD/MM/YYYY HH24:MI' -> 'dd/MM/yyyy HH:mm'
                 if (DateTime.TryParseExact(fecModStr, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out tmp))
                 {
                     fecModVal = tmp;
                 }
            }
            
            // Deserializar JSON de platos (Dapper puede devolverlo como string si es JSONB/JSON)
            // Aquí asumimos que viene como un objeto dynamic iterable si Dapper lo maneja, o string.
            // Simplificación: Asumimos que el Repo ya lo procesó o que es un dynamic.
            
            var platosList = new List<TipoPlatoCount>();
            // Nota: El parsing del JSON de PostgreSQL en Dapper puede requerir System.Text.Json
            // Si viene como string:
            if (item.Platos_por_tipo != null)
            {
                string json = item.Platos_por_tipo;
                try {
                     platosList = System.Text.Json.JsonSerializer.Deserialize<List<TipoPlatoCount>>(json)
                                 ?? new List<TipoPlatoCount>();
                } catch { /* Ignorar error de parseo */ }
            }

            response.Add(new MenuDiarioListItemResponse
            {
                Id = item.Id,
                Mendiafec = fechaStr,
                Codest = item.Codest,
                NombreFecha = GetFriendlyDateName(fechaVal),
                TiempoTranscurrido = GetTimeElapsed(fecModVal),
                CantidadEntradas = item.Cantidad_entradas ?? 0,
                CantidadPlatosPorTipo = platosList
            });
        }
        
        return response;
    }
    
    // Helpers de formato de fecha (para cuando implementemos el GetList real)
    private string GetFriendlyDateName(DateTime date)
    {
        var today = DateTime.Today;
        if (date.Date == today) return "Hoy";
        if (date.Date == today.AddDays(-1)) return "Ayer";
        if (date.Date == today.AddDays(1)) return "Mañana";

        // Misma semana? (Lunes a Domingo)
        var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
        var d1 = today.Date.AddDays(-1 * (int)cal.GetDayOfWeek(today));
        var d2 = date.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date));
        if (d1 == d2) return date.ToString("dddd"); // Lunes, Martes...

        return date.ToString("dd MMMM yyyy").ToUpper();
    }

    private string GetTimeElapsed(DateTime? lastMod)
    {
        if (!lastMod.HasValue) return "";
        var timeSpan = DateTime.Now - lastMod.Value;

        if (timeSpan.TotalMinutes < 60) return $"Hace {timeSpan.TotalMinutes:0} min";
        if (timeSpan.TotalHours < 24) return $"Hace {timeSpan.TotalHours:0} horas";
        if (timeSpan.TotalDays < 7) return $"Hace {timeSpan.TotalDays:0} días";
        if (timeSpan.TotalDays < 30) return $"Hace {(int)(timeSpan.TotalDays / 7)} semanas";
        
        return lastMod.Value.ToString("dd/MM/yyyy");
    }
}
