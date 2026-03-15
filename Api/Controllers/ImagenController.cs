using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Options;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ImagenController : ControllerBase
{
    private readonly IImagenService _imagenService;
    private readonly IStorageService _storageService;
    private readonly GcsOptions _gcsOptions;

    public ImagenController(IImagenService imagenService, IStorageService storageService, IOptions<GcsOptions> gcsOptions)
    {
        _imagenService = imagenService;
        _storageService = storageService;
        _gcsOptions = gcsOptions.Value;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _imagenService.GetByIdAsync(id, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? nombre, CancellationToken ct)
    {
        var result = await _imagenService.GetListAsync(nombre, ct);
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
        var objectName = $"images/{guid}{extension}";

        using var stream = file.OpenReadStream();
        await _storageService.UploadAsync(stream, objectName, contentTypeMap[extension], ct);

        var serviceRequest = new ImagenCreateServiceRequest
        {
            Imarut = objectName,
            Imanom = guid,
            Imaext = extension,
            Usureg = currentUser
        };

        var id = await _imagenService.CreateAsync(serviceRequest, ct);

        if (id > 0)
        {
            var created = await _imagenService.GetByIdAsync(id, ct);
            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        return BadRequest(new { detail = "Error al registrar la imagen." });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ImagenCreateRequest request, CancellationToken ct)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        
        var serviceRequest = new ImagenCreateServiceRequest
        {
            Imarut = request.Imarut,
            Imanom = request.Imanom,
            Imaext = request.Imaext,
            Usureg = currentUser
        };

        var id = await _imagenService.CreateAsync(serviceRequest, ct);
        
        if (id > 0)
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
            
        return BadRequest(); 
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ImagenUpdateRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("El ID de la URL no coincide con el cuerpo de la solicitud.");

        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";

        var serviceRequest = new ImagenUpdateServiceRequest
        {
            Id = request.Id,
            Imarut = request.Imarut,
            Imanom = request.Imanom,
            Imaext = request.Imaext,
            Codest = request.Codest,
            Usumod = currentUser
        };

        var success = await _imagenService.UpdateAsync(serviceRequest, ct);

        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var success = await _imagenService.DeleteAsync(id, ct);
        if (!success) return NotFound();

        return NoContent();
    }
}
