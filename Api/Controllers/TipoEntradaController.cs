using MenuSoda.Application.Dto;
using MenuSoda.Application.UseCases.TipoEntrada;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TipoEntradaController : ControllerBase
{
    private readonly ObtenerTipoEntradaPorIdUseCase _getByIdUseCase;
    private readonly ListarTiposEntradaUseCase _getListUseCase;
    private readonly CrearTipoEntradaUseCase _createUseCase;
    private readonly ActualizarTipoEntradaUseCase _updateUseCase;
    private readonly EliminarTipoEntradaUseCase _deleteUseCase;

    public TipoEntradaController(
        ObtenerTipoEntradaPorIdUseCase getByIdUseCase,
        ListarTiposEntradaUseCase getListUseCase,
        CrearTipoEntradaUseCase createUseCase,
        ActualizarTipoEntradaUseCase updateUseCase,
        EliminarTipoEntradaUseCase deleteUseCase)
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
    public async Task<IActionResult> GetList([FromQuery] string? descripcion, CancellationToken ct)
    {
        var result = await _getListUseCase.ExecuteAsync(descripcion, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TipoEntradaCreateRequest request, CancellationToken ct)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        var id = await _createUseCase.ExecuteAsync(request, currentUser, ct);
        
        if (id > 0)
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
            
        return BadRequest(); 
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TipoEntradaUpdateRequest request, CancellationToken ct)
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
