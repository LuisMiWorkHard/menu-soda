using MenuSoda.Application.Dto;
using MenuSoda.Application.Options;
using MenuSoda.Application.Utils;
using MenuSoda.Application.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    private readonly ObtenerRefreshTokenUseCase _refreshUseCase;
    private readonly LogoutUseCase _logoutUseCase;
    private readonly CambiarContrasenaUseCase _cambiarContrasenaUseCase;
    private readonly EnviarCodigoRecuperacionUseCase _enviarCodigoUseCase;
    private readonly VerificarCodigoRecuperacionUseCase _verificarCodigoUseCase;
    private readonly RestablecerContrasenaRecuperacionUseCase _restablecerContrasenaUseCase;
    private readonly ParseUtil _parseUtil;
    private readonly AuthOptions _authOptions;

    public AuthController(
        LoginUseCase loginUseCase,
        ObtenerRefreshTokenUseCase refreshUseCase,
        LogoutUseCase logoutUseCase,
        CambiarContrasenaUseCase cambiarContrasenaUseCase,
        EnviarCodigoRecuperacionUseCase enviarCodigoUseCase,
        VerificarCodigoRecuperacionUseCase verificarCodigoUseCase,
        RestablecerContrasenaRecuperacionUseCase restablecerContrasenaUseCase,
        ParseUtil parseUtil,
        IOptions<AuthOptions> authOptions)
    {
        _loginUseCase = loginUseCase;
        _refreshUseCase = refreshUseCase;
        _logoutUseCase = logoutUseCase;
        _cambiarContrasenaUseCase = cambiarContrasenaUseCase;
        _enviarCodigoUseCase = enviarCodigoUseCase;
        _verificarCodigoUseCase = verificarCodigoUseCase;
        _restablecerContrasenaUseCase = restablecerContrasenaUseCase;
        _parseUtil = parseUtil;
        _authOptions = authOptions.Value;
    }

    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();
        var deviceId = Request.Headers[_authOptions.DeviceIdHeaderName].ToString() ?? string.Empty;
        var (lat, lon) = _parseUtil.ParseGeo(Request.Headers[_authOptions.GeoLatHeaderName].ToString(), Request.Headers[_authOptions.GeoLonHeaderName].ToString());

        request.IpAddress = ipAddress;
        request.UserAgent = userAgent;
        request.DeviceId = deviceId;
        request.Latitud = lat;
        request.Longitud = lon;

        var result = await _loginUseCase.ExecuteAsync(request, ct);

        Response.Cookies.Append(_authOptions.RefreshTokenCookieName, result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = result.RefreshTokenExpiresUtc
        });

        return Ok(new { Token = result.AccessToken });
    }

    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var cookie = Request.Cookies[_authOptions.RefreshTokenCookieName];
        if (string.IsNullOrEmpty(cookie)) return Unauthorized();

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();
        var deviceId = Request.Headers[_authOptions.DeviceIdHeaderName].ToString() ?? string.Empty;
        var (lat, lon) = _parseUtil.ParseGeo(Request.Headers[_authOptions.GeoLatHeaderName].ToString(), Request.Headers[_authOptions.GeoLonHeaderName].ToString());

        var result = await _refreshUseCase.ExecuteAsync(
            new ObtenerRefreshTokenRequest
            {
                RefreshToken = cookie,
                IpAddress = ip,
                UserAgent = userAgent,
                DeviceId = deviceId,
                Latitud = lat,
                Longitud = lon
            }, ct);

        Response.Cookies.Append(_authOptions.RefreshTokenCookieName, result.RefreshTokenPlainText, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = result.RefreshTokenExpiresUtc
        });

        return Ok(new { Token = result.AccessToken });
    }

    [Authorize]
    [HttpPut("contrasena")]
    public async Task<IActionResult> CambiarContrasena(
        [FromBody] CambiarContrasenaRequest request,
        CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        await _cambiarContrasenaUseCase.ExecuteAsync(userId, request, ct);
        return NoContent();
    }

    [Authorize]
    [HttpPost("recuperar/codigo")]
    public async Task<IActionResult> EnviarCodigoRecuperacion(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var result = await _enviarCodigoUseCase.ExecuteAsync(userId, ct);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("recuperar/verificar")]
    public async Task<IActionResult> VerificarCodigoRecuperacion(
        [FromBody] VerificarCodigoRecuperacionRequest request,
        CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        await _verificarCodigoUseCase.ExecuteAsync(userId, request, ct);
        return NoContent();
    }

    [Authorize]
    [HttpPut("recuperar/contrasena")]
    public async Task<IActionResult> RestablecerContrasenaRecuperacion(
        [FromBody] RestablecerContrasenaRecuperacionRequest request,
        CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        await _restablecerContrasenaUseCase.ExecuteAsync(userId, request, ct);
        return NoContent();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var cookie = Request.Cookies[_authOptions.RefreshTokenCookieName];
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        await _logoutUseCase.ExecuteAsync(cookie, ipAddress, ct);

        Response.Cookies.Delete(_authOptions.RefreshTokenCookieName, new CookieOptions { Secure = true, SameSite = SameSiteMode.Strict, HttpOnly = true });
        return NoContent();
    }
}
