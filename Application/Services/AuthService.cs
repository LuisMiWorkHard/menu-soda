using MenuSoda.Application.Dto;
using MenuSoda.Application.Options;
using MenuSoda.Domain.Interfaces.Security;
using Microsoft.Extensions.Options;

namespace MenuSoda.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshToken _refreshTokens;
    private readonly AuthOptions _authOptions;

    public AuthService(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher, 
        ITokenGenerator tokenGenerator, 
        IRefreshToken refreshTokens,
        IOptions<AuthOptions> authOptions)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _refreshTokens = refreshTokens;
        _authOptions = authOptions.Value;
    }

    public async Task<LoginResponse> LoginAsync(LoginServiceRequest request, CancellationToken ct)
    {
        var user = await _userRepository.GetByDocumentAsync(new UsuarioGetByDocumentRequest
        {
            TipoDocumento = request.TipoDocumento,
            NumeroDocumento = request.NumeroDocumento
        },
        ct);

        if (user == null || !_passwordHasher.Verify(request.Contrasena, user.Usuhash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var accessToken = _tokenGenerator.GenerateToken(user);

        var refresh = await _refreshTokens.CreateAsync(new RefreshTokenCreateRequest
        {
            UserId = user.Id,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            DeviceId = request.DeviceId,
            GeoLat = request.Latitud,
            GeoLon = request.Longitud,
            DaysToExpire = _authOptions.RefreshTokenDaysToExpire
        }, ct);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refresh.PlainText,
            RefreshTokenExpiresUtc = refresh.ExpiresUtc
        };
    }

    public async Task<RefreshServiceResponse> RefreshAsync(
        RefreshServiceRequest request,
        CancellationToken ct)
    {
        var active = await _refreshTokens.GetActiveByPlainAsync(request.RefreshToken, ct) ?? throw new UnauthorizedAccessException("Token de refresco inválido.");
        
        var rotated = await _refreshTokens.RotateAsync(new RefreshTokenRotateRequest
        {
            OldTokenId = active.Id,
            UserId = active.UserId,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            DeviceId = request.DeviceId,
            GeoLat = request.Latitud,
            GeoLon = request.Longitud,
            DaysToExpire = _authOptions.RefreshTokenDaysToExpire
        }, ct);

        var user = await _userRepository.GetByIdAsync(new UsuarioGetByIdRequest
        {
            Id = active.UserId
        }, ct) ?? throw new UnauthorizedAccessException("Usuario no encontrado.");
        
        var accessToken = _tokenGenerator.GenerateToken(user);

        return new RefreshServiceResponse
        {
            AccessToken = accessToken,
            RefreshTokenPlainText = rotated.PlainText,
            RefreshTokenExpiresUtc = rotated.ExpiresUtc
        };
    }

    public async Task LogoutAsync(LogoutServiceRequest request, CancellationToken ct)
    {
        var token = await _refreshTokens.GetByPlainAsync(request.RefreshToken, ct);
        if (token != null)
        {
            await _refreshTokens.RevokeAllActiveAsync(token.UserId, request.IpAddress, ct);
        }
    }
}