namespace MenuSoda.Domain.Interfaces.Security
{
    public interface IRefreshToken
    {
        Task<RefreshTokenCreateResponse> CreateAsync(RefreshTokenCreateRequest request);
        Task<RefreshTokenRow?> GetByPlainAsync(string plainText); // trae incluso revocados/expirados
        Task<(string newPlainText, DateTime newExpiresUtc, Guid newTokenId)> RotateAsync(Guid oldTokenId, Guid userId, string? ip, int daysToExpire = 14);
        Task RevokeAllActiveAsync(Guid userId, string? ip);
    }
}