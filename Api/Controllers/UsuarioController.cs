using MenuSoda.Application.UseCases.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MenuSoda.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly ObtenerUsuarioPorIdUseCase _obtenerUsuario;

    public UsuarioController(ObtenerUsuarioPorIdUseCase obtenerUsuario)
    {
        _obtenerUsuario = obtenerUsuario;
    }

    [HttpGet]
    public async Task<IActionResult> GetById(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var result = await _obtenerUsuario.ExecuteAsync(userId, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
