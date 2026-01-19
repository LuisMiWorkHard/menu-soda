using MenuSoda.Application.Dto;
using MenuSoda.Application.Options;
using MenuSoda.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly AuthService _authService;
    private readonly UtilService _utilService;
    private readonly AuthOptions _authOptions;

    public AuthController(
        AuthService authService, 
        UtilService utilService, 
        IOptions<AuthOptions> authOptions)
    {
        _authService = authService;
        _utilService = utilService;
        _authOptions = authOptions.Value;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();
        var deviceId = Request.Headers[_authOptions.DeviceIdHeaderName].ToString() ?? string.Empty;
        var (lat, lon) = _utilService.ParseGeo(Request.Headers[_authOptions.GeoLatHeaderName].ToString(), Request.Headers[_authOptions.GeoLonHeaderName].ToString());

        var result = await _authService.LoginAsync(new LoginServiceRequest
        {
            TipoDocumento = request.TipoDocumento,
            NumeroDocumento = request.NumeroDocumento,
            Contrasena = request.Contrasena,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            DeviceId = deviceId,
            Latitud = lat,
            Longitud = lon
        }, ct);

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
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var cookie = Request.Cookies[_authOptions.RefreshTokenCookieName];
        if (string.IsNullOrEmpty(cookie)) return Unauthorized();

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();
        var deviceId = Request.Headers[_authOptions.DeviceIdHeaderName].ToString() ?? string.Empty;
        var (lat, lon) = _utilService.ParseGeo(Request.Headers[_authOptions.GeoLatHeaderName].ToString(), Request.Headers[_authOptions.GeoLonHeaderName].ToString());

        var result = await _authService.RefreshAsync(
            new RefreshServiceRequest
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
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var cookie = Request.Cookies[_authOptions.RefreshTokenCookieName];
        if (!string.IsNullOrEmpty(cookie))
        {
            await _authService.LogoutAsync(new LogoutServiceRequest
            {
                RefreshToken = cookie,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            }, ct);
        }

        Response.Cookies.Delete(_authOptions.RefreshTokenCookieName, new CookieOptions { Secure = true, SameSite = SameSiteMode.Strict, HttpOnly = true });
        return NoContent();
    }
}