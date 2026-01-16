using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MenuSoda.Application.Dto;
using MenuSoda.Application.Services;
using MenuSoda.Domain.Interfaces.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly AuthService _authService;
    private readonly IRefreshToken _refreshTokens;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthController(AuthService authService, IRefreshToken refreshTokens, ITokenGenerator tokenGenerator)
    {
        _authService = authService;
        _refreshTokens = refreshTokens;
        _tokenGenerator = tokenGenerator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var accessToken = await _authService.LoginAsync(request);
        if (accessToken == null) return Unauthorized();

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        RefreshTokenCreateResponse response = await _refreshTokens.CreateAsync(new RefreshTokenCreateRequest
        {
            UserId = int.Parse(userIdClaim.Value),
            IpAddress = ip,
            UserAgent = Request.Headers.UserAgent.ToString(),
            DeviceId = Request.Headers["DeviceId"].ToString(),
            GeoLat = request.Latitud,
            GeoLon = request.Longitud,
            DaysToExpire = 15
        });

        Response.Cookies.Append("refresh_token", response.PlainText, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = response.ExpiresUtc
        });

        return Ok(new { Token = accessToken });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var cookie = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(cookie)) return Unauthorized();

        var active = await _refreshTokens.GetActiveByPlainAsync(cookie);
        if (active is null) return Unauthorized();

        Guid oldTokenId = active.Id;
        int userId = active.UserId;
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var newRefreshToken = await _refreshTokens.RotateAsync(new RefreshTokenRotateRequest
        {
            OldTokenId = oldTokenId,
            UserId = userId,
            IpAddress = ip,
            UserAgent = Request.Headers.UserAgent.ToString(),
            DeviceId = Request.Headers["DeviceId"].ToString(),
            DaysToExpire = 15
        });

        // Cargar usuario para emitir nuevo access token
        var user = await _authService.GetUserByIdAsync(userId);
        if (user is null) return Unauthorized();

        var newAccess = _tokenGenerator.GenerateToken(user);

        Response.Cookies.Append("refresh_token", newRefreshToken.PlainText, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = newRefreshToken.ExpiresUtc
        });

        return Ok(new { Token = newAccess });
    }

    /*
    private readonly IRefreshTokenService _refreshTokens;
    private readonly ITokenGenerator _tokenGenerator;
    // ...existing code...

    public AuthController(AuthService authService, IRefreshTokenService refreshTokens, ITokenGenerator tokenGenerator)
    {
        _authService = authService;
        _refreshTokens = refreshTokens;
        _tokenGenerator = tokenGenerator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(request);
        if (token == null) return Unauthorized();

        // Obtener userId desde el JWT (claim "sub")
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var userId = Guid.Parse(userIdClaim.Value);
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var (plain, expiresUtc, _) = await _refreshTokens.CreateAsync(userId, ip, daysToExpire: 14);

        Response.Cookies.Append("refresh_token", plain, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresUtc
        });

        return Ok(new { Token = token });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var cookie = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(cookie)) return Unauthorized();

        var active = await _refreshTokens.GetActiveByPlainAsync(cookie);
        if (active is null) return Unauthorized();

        var (oldTokenId, userId) = active.Value;
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var (newPlain, newExpires, _) = await _refreshTokens.RotateAsync(oldTokenId, userId, ip, daysToExpire: 14);

        // Cargar usuario para emitir nuevo access token
        var user = await _authService.GetUserByIdAsync(userId);
        if (user is null) return Unauthorized();

        var newAccess = _tokenGenerator.GenerateToken(user);

        Response.Cookies.Append("refresh_token", newPlain, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = newExpires
        });

        return Ok(new { Token = newAccess });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var cookie = Request.Cookies["refresh_token"];
        if (!string.IsNullOrEmpty(cookie))
        {
            var active = await _refreshTokens.GetActiveByPlainAsync(cookie);
            if (active is not null)
            {
                var (_, userId) = active.Value;
                await _refreshTokens.RevokeAllActiveAsync(userId, HttpContext.Connection.RemoteIpAddress?.ToString());
            }
        }

        Response.Cookies.Delete("refresh_token", new CookieOptions { Secure = true, SameSite = SameSiteMode.Strict, HttpOnly = true });
        return NoContent();
    }
    */
}