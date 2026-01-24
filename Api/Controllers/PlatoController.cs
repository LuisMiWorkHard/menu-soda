using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PlatoController : ControllerBase
{
    private readonly IPlatoService _platoService;

    public PlatoController(IPlatoService platoService)
    {
        _platoService = platoService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _platoService.GetByIdAsync(id, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] string? nombre, CancellationToken ct)
    {
        var result = await _platoService.GetListAsync(nombre, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PlatoCreateRequest request, CancellationToken ct)
    {
        try 
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var id = await _platoService.CreateAsync(request, currentUser, ct);
            
            if (id > 0)
                return CreatedAtAction(nameof(GetById), new { id }, new { id });
                
            return BadRequest(); 
        }
        catch (Exception ex)
        {
            // Capturar excepción de DB (integridad referencial que añadimos al SP)
            if (ex.Message.Contains("no existe o no está activo"))
                return UnprocessableEntity(new { message = ex.Message });
                
            throw;
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PlatoUpdateRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("El ID de la URL no coincide con el cuerpo de la solicitud.");

        try
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var success = await _platoService.UpdateAsync(request, currentUser, ct);

            if (!success) return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
             // Capturar excepción de DB (integridad referencial que añadimos al SP)
            if (ex.Message.Contains("no existe o no está activo"))
                 return UnprocessableEntity(new { message = ex.Message });
                 
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var success = await _platoService.DeleteAsync(id, ct);
        if (!success) return NotFound();

        return NoContent();
    }
}
