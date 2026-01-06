using MenuSoda.Application.Dto;

namespace MenuSoda.Domain.Interfaces.Security
{
    public interface IRefreshToken
    {
        Task<RefreshTokenCreateResponse> CreateAsync(RefreshTokenCreateRequest request);
        Task<RefreshTokenGetByHashResponse?> GetByPlainAsync(string plainText); // trae incluso revocados/expirados
        Task<RefreshTokenRotateResponse> RotateAsync(RefreshTokenRotateRequest request);
        Task RevokeAllActiveAsync(int userId, string? ip);
    }
}