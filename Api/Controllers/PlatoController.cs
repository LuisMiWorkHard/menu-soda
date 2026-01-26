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
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        
        var serviceRequest = new PlatoCreateServiceRequest
        {
            Planom = request.Planom,
            Plades = request.Plades,
            Codtippla = request.Codtippla,
            Usureg = currentUser
        };

        var id = await _platoService.CreateAsync(serviceRequest, ct);
        
        if (id > 0)
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
            
        return BadRequest(); 
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PlatoUpdateRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("El ID de la URL no coincide con el cuerpo de la solicitud.");

        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        
        var serviceRequest = new PlatoUpdateServiceRequest
        {
            Id = request.Id,
            Planom = request.Planom,
            Plades = request.Plades,
            Codtippla = request.Codtippla,
            Codest = request.Codest,
            Usumod = currentUser
        };

        var success = await _platoService.UpdateAsync(serviceRequest, ct);

        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var success = await _platoService.DeleteAsync(id, ct);
        if (!success) return NotFound();

        return NoContent();
    }
}
