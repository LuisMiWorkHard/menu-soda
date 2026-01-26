using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TipoEntradaController : ControllerBase
{
    private readonly ITipoEntradaService _tipoEntradaService;

    public TipoEntradaController(ITipoEntradaService tipoEntradaService)
    {
        _tipoEntradaService = tipoEntradaService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _tipoEntradaService.GetByIdAsync(id, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? descripcion, CancellationToken ct)
    {
        var result = await _tipoEntradaService.GetListAsync(descripcion, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TipoEntradaCreateRequest request, CancellationToken ct)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        
        var serviceRequest = new TipoEntradaCreateServiceRequest
        {
            Tipentdes = request.Tipentdes,
            Usureg = currentUser
        };

        var id = await _tipoEntradaService.CreateAsync(serviceRequest, ct);
        
        if (id > 0)
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
            
        return BadRequest(); 
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TipoEntradaUpdateRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("El ID de la URL no coincide con el cuerpo de la solicitud.");

        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";

        var serviceRequest = new TipoEntradaUpdateServiceRequest
        {
            Id = request.Id,
            Tipentdes = request.Tipentdes,
            Codest = request.Codest,
            Usumod = currentUser
        };

        var success = await _tipoEntradaService.UpdateAsync(serviceRequest, ct);

        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var success = await _tipoEntradaService.DeleteAsync(id, ct);
        if (!success) return NotFound();

        return NoContent();
    }
}
