using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenuSoda.Application.Dto;
using MenuSoda.Application.UseCases.MenuDiario;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/menu-diario")]
public class MenuDiarioController : ControllerBase
{
    private readonly ObtenerMenuDiarioPorIdUseCase _getByIdUseCase;
    private readonly ListarMenusDiariosUseCase _getListUseCase;
    private readonly CrearMenuDiarioUseCase _createUseCase;
    private readonly ActualizarMenuDiarioUseCase _updateUseCase;
    private readonly EliminarMenuDiarioUseCase _deleteUseCase;

    public MenuDiarioController(
        ObtenerMenuDiarioPorIdUseCase getByIdUseCase,
        ListarMenusDiariosUseCase getListUseCase,
        CrearMenuDiarioUseCase createUseCase,
        ActualizarMenuDiarioUseCase updateUseCase,
        EliminarMenuDiarioUseCase deleteUseCase)
    {
        _getByIdUseCase = getByIdUseCase;
        _getListUseCase = getListUseCase;
        _createUseCase = createUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _getByIdUseCase.ExecuteAsync(id, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? busqueda, CancellationToken ct)
    {
        var result = await _getListUseCase.ExecuteAsync(busqueda, ct);
        return Ok(result);
    }

    [HttpPost, Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(
        [FromForm] MenuDiarioCreateRequest request,
        [FromForm] IFormFile? imagen,
        CancellationToken ct)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        var id = await _createUseCase.ExecuteAsync(request, imagen, currentUser, ct);

        if (id > 0)
            return CreatedAtAction(nameof(GetById), new { id }, new { id });

        return BadRequest();
    }

    [HttpPut("{id}"), Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(int id, [FromForm] MenuDiarioUpdateRequest request, IFormFile? imagen, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("El ID de la URL no coincide con el cuerpo de la solicitud.");

        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        var success = await _updateUseCase.ExecuteAsync(request, imagen, currentUser, ct);

        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        var success = await _deleteUseCase.ExecuteAsync(id, currentUser, ct);
        if (!success) return NotFound();

        return NoContent();
    }
}
