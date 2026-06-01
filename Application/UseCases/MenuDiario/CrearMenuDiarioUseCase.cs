using MenuSoda.Domain.Exceptions;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Options;
using MenuSoda.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

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
    private readonly IStorageService _storageService;
    private readonly GcsOptions _gcsOptions;
    private readonly DapperContext _dapperContext;
    private readonly ILogger<CrearMenuDiarioUseCase> _logger;

    private static readonly HashSet<string> _extensionesPermitidas =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

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
        IStorageService storageService,
        IOptions<GcsOptions> gcsOptions,
        ILogger<CrearMenuDiarioUseCase> logger,
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
        _storageService = storageService;
        _gcsOptions = gcsOptions.Value;
        _logger = logger;
        _dapperContext = dapperContext;
    }

    public async Task<int> ExecuteAsync(MenuDiarioCreateRequest request, IFormFile? imagen, string currentUser, CancellationToken ct)
    {
        // 1. Validar entradas y platos existen en DB
        foreach (var entId in request.EntradasIds)
        {
            var ent = await _entradaRepository.GetByIdAsync(entId, ct);
            if (ent == null || ent.Codest == 0)
                throw new CustomBusinessValidationException($"La entrada no existe o no está activa.");
        }

        foreach (var platoItem in request.Platos)
        {
            var pla = await _platoRepository.GetByIdAsync(platoItem.PlatoId, ct);
            if (pla == null || pla.Codest == 0)
                throw new CustomBusinessValidationException($"El plato no existe o no está activo.");

            if (platoItem.AdicionalId.HasValue && platoItem.AdicionalId.Value > 0)
            {
                var adi = await _adicionalRepository.GetByIdAsync(platoItem.AdicionalId.Value, ct);
                if (adi == null || adi.Codest == 0)
                    throw new CustomBusinessValidationException($"El adicional no existe o no está activo.");
            }
        }

        // 2. Validar imagen si se proporcionó
        string? objectName = null;
        string? extension = null;
        if (imagen != null)
        {
            extension = Path.GetExtension(imagen.FileName);
            if (!_extensionesPermitidas.Contains(extension))
                throw new CustomBusinessValidationException(
                    $"La extensión '{extension}' no está permitida. Use: {string.Join(", ", _extensionesPermitidas)}.");

            var maxBytes = (long)_gcsOptions.MaxImageSizeMb * 1024 * 1024;
            if (imagen.Length > maxBytes)
                throw new CustomBusinessValidationException(
                    $"La imagen supera el tamaño máximo permitido de {_gcsOptions.MaxImageSizeMb} MB.");

            // 3. Generar nombre del objeto en GCS
            objectName = $"cartas/{Guid.NewGuid()}{extension}";

            // 4. Subir a GCS
            using var stream = imagen.OpenReadStream();
            await _storageService.UploadAsync(stream, objectName, imagen.ContentType, ct);
        }

        // 5. Transacción DB con compensación si falla
        using var connection = _dapperContext.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // A. Insertar imagen en DB si se subió a GCS
            int? imagenId = null;
            if (objectName != null)
            {
                imagenId = await _imagenRepository.CreateAsync(new ImagenCreateRequest
                {
                    Ruta = objectName,
                    Nombre = Path.GetFileNameWithoutExtension(imagen!.FileName),
                    Extension = extension!
                }, currentUser, ct, transaction);
            }

            // B. Insertar header del menú
            var menuId = await _menuDiarioRepository.CreateAsync(request, currentUser, ct, transaction);

            // C. Insertar relación menu-imagen
            if (imagenId.HasValue)
            {
                await _menuDiarioImagenRepository.AddAsync(new MenuDiarioImagenInsertRequest
                {
                    MenuDiarioId  = menuId,
                    ImagenId      = imagenId.Value,
                    MenuImagenId  = request.MenuImagenId,
                    UsuarioRegistro = currentUser
                }, ct, transaction);
            }

            // D. Insertar entradas
            for (var i = 0; i < request.EntradasIds.Count; i++)
            {
                await _menuDiarioEntradaRepository.AddAsync(new MenuDiarioEntradaInsertRequest
                {
                    MenuDiarioId = menuId,
                    EntradaId = request.EntradasIds[i],
                    UsuarioRegistro = currentUser,
                    Orden = i
                }, ct, transaction);
            }

            // E. Insertar platos y adicionales
            for (var i = 0; i < request.Platos.Count; i++)
            {
                var platoItem = request.Platos[i];
                var idPlatoGenerado = await _menuDiarioPlatoRepository.AddAsync(new MenuDiarioPlatoInsertRequest
                {
                    Codmendia = menuId,
                    Codpla = platoItem.PlatoId,
                    Usureg = currentUser,
                    Orden = i
                }, ct, transaction);

                if (platoItem.AdicionalId.HasValue && platoItem.AdicionalId.Value > 0)
                {
                    await _menuDiarioPlatoAdicionalRepository.AddAsync(new MenuDiarioPlatoAdicionalInsertRequest
                    {
                        MenuDiarioPlatoId = idPlatoGenerado,
                        AdicionalId = platoItem.AdicionalId.Value,
                        UsuarioRegistro = currentUser
                    }, ct, transaction);
                }
            }

            transaction.Commit();
            return menuId;
        }
        catch
        {
            transaction.Rollback();

            // Compensación: eliminar objeto de GCS si ya fue subido
            if (objectName != null)
            {
                try { await _storageService.DeleteAsync(objectName, ct); }
                catch (Exception exGcs)
                {
                    _logger.LogError(exGcs,
                        "No fue posible eliminar la imagen del repositorio GCS. ObjectName: {ObjectName}",
                        objectName);
                }
            }

            throw;
        }
    }
}
