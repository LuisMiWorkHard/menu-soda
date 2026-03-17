using MenuSoda.Application.Dto;
using MenuSoda.Application.UseCases.Entrada;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EntradaController : ControllerBase
{
    private readonly ObtenerEntradaPorIdUseCase _obtenerEntradaPorId;
    private readonly ListarEntradasUseCase _listarEntradas;
    private readonly CrearEntradaUseCase _crearEntrada;
    private readonly ActualizarEntradaUseCase _actualizarEntrada;
    private readonly EliminarEntradaUseCase _eliminarEntrada;

    public EntradaController(
        ObtenerEntradaPorIdUseCase obtenerEntradaPorId,
        ListarEntradasUseCase listarEntradas,
        CrearEntradaUseCase crearEntrada,
        ActualizarEntradaUseCase actualizarEntrada,
        EliminarEntradaUseCase eliminarEntrada)
    {
        _obtenerEntradaPorId = obtenerEntradaPorId;
        _listarEntradas = listarEntradas;
        _crearEntrada = crearEntrada;
        _actualizarEntrada = actualizarEntrada;
        _eliminarEntrada = eliminarEntrada;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? filter, CancellationToken ct)
    {
        var result = await _listarEntradas.ExecuteAsync(filter, ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _obtenerEntradaPorId.ExecuteAsync(id, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EntradaCreateRequest request, CancellationToken ct)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var id = await _crearEntrada.ExecuteAsync(request, currentUser, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] EntradaUpdateRequest request, CancellationToken ct)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var success = await _actualizarEntrada.ExecuteAsync(request, currentUser, ct);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var success = await _eliminarEntrada.ExecuteAsync(id, ct);
        if (!success) return NotFound();
        return NoContent();
    }
}
