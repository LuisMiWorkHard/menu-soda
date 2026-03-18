using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenuSoda.Application.Dto;
using MenuSoda.Application.UseCases.TipoPlato;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TipoPlatoController : ControllerBase
{
    private readonly ObtenerTipoPlatoPorIdUseCase _getByIdUseCase;
    private readonly ListarTiposPlatoUseCase _getListUseCase;
    private readonly CrearTipoPlatoUseCase _createUseCase;
    private readonly ActualizarTipoPlatoUseCase _updateUseCase;
    private readonly EliminarTipoPlatoUseCase _deleteUseCase;

    public TipoPlatoController(
        ObtenerTipoPlatoPorIdUseCase getByIdUseCase,
        ListarTiposPlatoUseCase getListUseCase,
        CrearTipoPlatoUseCase createUseCase,
        ActualizarTipoPlatoUseCase updateUseCase,
        EliminarTipoPlatoUseCase deleteUseCase)
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
    public async Task<IActionResult> Create([FromBody] TipoPlatoCreateRequest request, CancellationToken ct)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        var id = await _createUseCase.ExecuteAsync(request, currentUser, ct);

        if (id > 0)
            return CreatedAtAction(nameof(GetById), new { id }, new { id });

        return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TipoPlatoUpdateRequest request, CancellationToken ct)
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
