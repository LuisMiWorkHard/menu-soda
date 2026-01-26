using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ImagenController : ControllerBase
{
    private readonly IImagenService _imagenService;

    public ImagenController(IImagenService imagenService)
    {
        _imagenService = imagenService;
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
