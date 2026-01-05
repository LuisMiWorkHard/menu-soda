using System.Data;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Security;
using Npgsql;

public class DapperRefreshTokenService : IRefreshToken
{
    private readonly string _cs;
    public DapperRefreshTokenService(
        IConfiguration cfg
    ){
        _cs = cfg.GetConnectionString("URL_SERVER")!;
    } 

    public async Task<RefreshTokenCreateResponse> CreateAsync(RefreshTokenCreateRequest request)
    {
        using var conn = new NpgsqlConnection(_cs);
        var plain = GenerateToken(64);
        var hash = Hash(plain);

        var res = await conn.QuerySingleAsync<(Guid Id, DateTime ExpiresAtUtc)>(
            "seguridad.sp_create_refreshtoken",
            new { 
                p_usuid = request.UserId,
                p_tokenhash = hash, 
                p_ipcre = request.IpAddress, 
                p_useragent = request.UserAgent, 
                p_deviceid = request.DeviceId, 
                p_geolat = request.GeoLat, 
                p_geolon = request.GeoLon, 
                p_days_to_expire = request.DaysToExpire 
                },
            commandType: CommandType.StoredProcedure);

        return new RefreshTokenCreateResponse { 
            PlainText = plain,
            ExpiresUtc = res.ExpiresAtUtc, 
            TokenId = res.Id 
        };
    }

    public Task<RefreshTokenGetByHashRequest?> GetByPlainAsync(string plainText)
    {
        /*SELECT id, usuid, hash, feccreutc, ipcre, fecexputc,
           fecrevoutc, iprev, reftokidref, useragent, deviceid,
           geolat, geolon
        FROM seguridad.refreshtoken*/

        using var conn = new SqlConnection(_cs);
        var hash = Hash(plainText);
        return await conn.QueryFirstOrDefaultAsync<RefreshTokenRow>(
            "dbo.sp_RefreshToken_GetByHash",
            new { TokenHash = hash },
            commandType: CommandType.StoredProcedure);
    }

    public Task RevokeAllActiveAsync(Guid userId, string? ip)
    {
        throw new NotImplementedException();
    }

    public Task<(string newPlainText, DateTime newExpiresUtc, Guid newTokenId)> RotateAsync(Guid oldTokenId, Guid userId, string? ip, int daysToExpire = 14)
    {
        throw new NotImplementedException();
    }

    private static string GenerateToken(int bytes) => Convert.          ToBase64String(RandomNumberGenerator.GetBytes(bytes));
    private static string Hash(string s) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(s)));
}