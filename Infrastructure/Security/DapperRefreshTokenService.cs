using MenuSoda.Domain.Interfaces.Security;

public class DapperRefreshTokenService : IRefreshToken
{
    public Task<RefreshTokenCreateResponse> CreateAsync(RefreshTokenCreateRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshTokenRow?> GetByPlainAsync(string plainText)
    {
        throw new NotImplementedException();
    }

    public Task RevokeAllActiveAsync(Guid userId, string? ip)
    {
        throw new NotImplementedException();
    }

    public Task<(string newPlainText, DateTime newExpiresUtc, Guid newTokenId)> RotateAsync(Guid oldTokenId, Guid userId, string? ip, int daysToExpire = 14)
    {
        throw new NotImplementedException();
    }
}