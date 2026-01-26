using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/menu-diario")] // Kebab-case por convención
public class MenuDiarioController : ControllerBase
{
    private readonly IMenuDiarioService _menuDiarioService;

    public MenuDiarioController(IMenuDiarioService menuDiarioService)
    {
        _menuDiarioService = menuDiarioService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _menuDiarioService.GetByIdAsync(id, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList(CancellationToken ct)
    {
        var result = await _menuDiarioService.GetListAsync(ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MenuDiarioCreateRequest request, CancellationToken ct)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        
        var serviceRequest = new MenuDiarioCreateServiceRequest
        {
            Mendiafec = request.Mendiafec,
            EntradasIds = request.EntradasIds,
            // PlatosIds ya no se usa, mapeamos Platos directo
            Platos = request.Platos,
            ImagenId = request.ImagenId,
            Usureg = currentUser
        };

        var id = await _menuDiarioService.CreateAsync(serviceRequest, ct);

        if (id > 0)
            return CreatedAtAction(nameof(GetById), new { id }, new { id });

        return BadRequest(); 
        // Nota: Si el servicio lanza CustomBusinessValidationException, 
        // el GlobalExceptionHandler lo capturará y devolverá 422, así que aquí create está limpio.
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] MenuDiarioUpdateRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("El ID de la URL no coincide con el cuerpo de la solicitud.");

        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        
        var serviceRequest = new MenuDiarioUpdateServiceRequest
        {
            Id = request.Id,
            Mendiafec = request.Mendiafec,
            Codest = request.Codest,
            EntradasIds = request.EntradasIds,
            Platos = request.Platos,
            ImagenId = request.ImagenId,
            Usumod = currentUser
        };

        var success = await _menuDiarioService.UpdateAsync(serviceRequest, ct);

        if (!success) return NotFound(); // O el servicio lanzaría excepción si el ID no existe?
        // En nuestra implementación, el servicio lanza CustomBusinessException si no existe.
        // Si llegamos aquí con false, algo muy raro pasó.

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown";
        var success = await _menuDiarioService.DeleteAsync(id, currentUser, ct);
        if (!success) return NotFound();

        return NoContent();
    }
}
