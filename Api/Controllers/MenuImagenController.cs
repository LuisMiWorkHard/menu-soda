using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Options;
using MenuSoda.Application.UseCases.Imagen;
using MenuSoda.Application.UseCases.MenuImagen;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MenuImagenController : ControllerBase
{
    private readonly ObtenerMenuImagenPorIdUseCase _getByIdUseCase;
    private readonly ListarMenuImagenesUseCase _getListUseCase;
    private readonly CrearMenuImagenUseCase _createUseCase;
    private readonly ActualizarMenuImagenUseCase _updateUseCase;
    private readonly EliminarMenuImagenUseCase _deleteUseCase;
    private readonly IStorageService _storageService;
    private readonly GcsOptions _gcsOptions;
    private readonly CrearImagenUseCase _createImagenUseCase;
    private readonly IHttpClientFactory _httpClientFactory;

    public MenuImagenController(
        ObtenerMenuImagenPorIdUseCase getByIdUseCase,
        ListarMenuImagenesUseCase getListUseCase,
        CrearMenuImagenUseCase createUseCase,
        ActualizarMenuImagenUseCase updateUseCase,
        EliminarMenuImagenUseCase deleteUseCase,
        IStorageService storageService,
        IOptions<GcsOptions> gcsOptions,
        CrearImagenUseCase createImagenUseCase,
        IHttpClientFactory httpClientFactory)
    {
        _getByIdUseCase = getByIdUseCase;
        _getListUseCase = getListUseCase;
        _createUseCase = createUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
        _storageService = storageService;
        _gcsOptions = gcsOptions.Value;
        _createImagenUseCase = createImagenUseCase;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _getByIdUseCase.ExecuteAsync(id, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("{id}/contenido")]
    public async Task<IActionResult> GetContenido(int id, CancellationToken ct)
    {
        var result = await _getByIdUseCase.ExecuteAsync(id, ct);
        if (result == null) return NotFound();

        var signedUrl = _storageService.GetSignedUrl(result.ImagenRuta);
        var httpClient = _httpClientFactory.CreateClient();
        var gcsResponse = await httpClient.GetAsync(signedUrl, ct);
        if (!gcsResponse.IsSuccessStatusCode) return NotFound();

        var contentType = result.ImagenExtension?.ToLower() switch
        {
            ".png"  => "image/png",
            ".webp" => "image/webp",
            _       => "image/jpeg"
        };

        var stream = await gcsResponse.Content.ReadAsStreamAsync(ct);
        return File(stream, contentType);
    }

    [HttpGet]
    public async Task<IActionResult> GetList(CancellationToken ct)
    {
        var result = await _getListUseCase.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpPost("upload")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { detail = "No se proporcionó un archivo válido." });

        var maxBytes = _gcsOptions.MaxImageSizeMb * 1024L * 1024;
        if (file.Length > maxBytes)
            return BadRequest(new { detail = $"El archivo excede el tamaño máximo permitido ({_gcsOptions.MaxImageSizeMb} MB)." });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { detail = "Extensión no permitida. Use: jpg, jpeg, png o webp." });

        var contentTypeMap = new Dictionary<string, string>
        {
            { ".jpg",  "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png",  "image/png"  },
            { ".webp", "image/webp" }
        };

        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        var guid = Guid.NewGuid().ToString();
        var objectName = $"backgrounds/{guid}{extension}";

        using var stream = file.OpenReadStream();
        await _storageService.UploadAsync(stream, objectName, contentTypeMap[extension], ct);

        var imagenRequest = new ImagenCreateRequest
        {
            Ruta = objectName,
            Nombre = guid,
            Extension = extension
        };

        var imagenId = await _createImagenUseCase.ExecuteAsync(imagenRequest, currentUser, ct);

        if (imagenId <= 0)
            return BadRequest(new { detail = "Error al registrar la imagen." });

        var menuImagenRequest = new MenuImagenCreateRequest { ImagenId = imagenId };
        var id = await _createUseCase.ExecuteAsync(menuImagenRequest, currentUser, ct);

        if (id > 0)
        {
            var created = await _getByIdUseCase.ExecuteAsync(id, ct);
            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        return BadRequest(new { detail = "Error al registrar la imagen del menú." });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MenuImagenCreateRequest request, CancellationToken ct)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        var id = await _createUseCase.ExecuteAsync(request, currentUser, ct);

        if (id > 0)
        {
            var created = await _getByIdUseCase.ExecuteAsync(id, ct);
            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] MenuImagenUpdateRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("El ID de la URL no coincide con el cuerpo de la solicitud.");

        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        var success = await _updateUseCase.ExecuteAsync(request, currentUser, ct);

        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var success = await _deleteUseCase.ExecuteAsync(id, ct);
        if (!success) return NotFound();
        return NoContent();
    }
}
