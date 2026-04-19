using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.UseCases.MenuImagen;
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
    private readonly IHttpClientFactory _httpClientFactory;

    public MenuImagenController(
        ObtenerMenuImagenPorIdUseCase getByIdUseCase,
        ListarMenuImagenesUseCase getListUseCase,
        CrearMenuImagenUseCase createUseCase,
        ActualizarMenuImagenUseCase updateUseCase,
        EliminarMenuImagenUseCase deleteUseCase,
        IStorageService storageService,
        IHttpClientFactory httpClientFactory)
    {
        _getByIdUseCase = getByIdUseCase;
        _getListUseCase = getListUseCase;
        _createUseCase = createUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
        _storageService = storageService;
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
