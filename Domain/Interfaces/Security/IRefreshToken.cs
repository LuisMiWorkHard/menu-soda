using MenuSoda.Domain.Models.Security;

namespace MenuSoda.Domain.Interfaces.Security
{
    public interface IRefreshToken
    {
        Task<RefreshTokenCreateResponse> CreateAsync(RefreshTokenCreateRequest request, CancellationToken ct);
        Task<RefreshTokenGetByHashResponse?> GetActiveByPlainAsync(string plainText, CancellationToken ct); 
        Task<RefreshTokenGetByHashResponse?> GetByPlainAsync(string plainText, CancellationToken ct); // trae incluso revocados/expirados
        Task<RefreshTokenRotateResponse> RotateAsync(RefreshTokenRotateRequest request, CancellationToken ct);
        Task RevokeAllActiveAsync(int userId, string? ip, CancellationToken ct);
    }
}