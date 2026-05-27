using MenuSoda.Application.UseCases.Perfil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PerfilController : ControllerBase
{
    private readonly ObtenerPerfilUseCase _obtenerPerfil;

    public PerfilController(ObtenerPerfilUseCase obtenerPerfil)
    {
        _obtenerPerfil = obtenerPerfil;
    }

    [HttpGet]
    public async Task<IActionResult> GetPerfil(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var result = await _obtenerPerfil.ExecuteAsync(userId, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
