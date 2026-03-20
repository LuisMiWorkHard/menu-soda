using MenuSoda.Application.Dto;
using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Interfaces;

public interface IRefreshToken
{
    Task<RefreshTokenCreateResponse> CreateAsync(User user, LoginRequest request, int daysToExpire, CancellationToken ct);
    Task<RefreshTokenGetByHashResponse?> GetActiveByPlainAsync(string plainText, CancellationToken ct);
    Task<RefreshTokenGetByHashResponse?> GetByPlainAsync(string plainText, CancellationToken ct);
    Task<RefreshTokenRotateResponse> RotateAsync(RefreshTokenGetByHashResponse tokenInfo, ObtenerRefreshTokenRequest request, int daysToExpire, CancellationToken ct);
    Task RevokeAllActiveAsync(int userId, string? ip, CancellationToken ct);
    Task RevokeAsync(string plainText, string? ip, CancellationToken ct);
}
