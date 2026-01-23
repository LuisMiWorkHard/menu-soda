using MenuSoda.Application.Dto;
using MenuSoda.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EntradaController : ControllerBase
{
    private readonly IEntradaService _entradaService;

    public EntradaController(IEntradaService entradaService)
    {
        _entradaService = entradaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? filter, CancellationToken ct)
    {
        var result = await _entradaService.GetListAsync(filter, ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _entradaService.GetByIdAsync(id, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EntradaCreateRequest request, CancellationToken ct)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var id = await _entradaService.CreateAsync(request, currentUser, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] EntradaUpdateRequest request, CancellationToken ct)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var success = await _entradaService.UpdateAsync(request, currentUser, ct);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var success = await _entradaService.DeleteAsync(id, ct);
        if (!success) return NotFound();
        return NoContent();
    }
}
