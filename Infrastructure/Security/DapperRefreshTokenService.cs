using System.Data;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Security;
using Npgsql;

public class DapperRefreshTokenService : IRefreshToken
{
    //private readonly string _cs;
    private readonly GenericRepository _genericRepository;

    public DapperRefreshTokenService(
        //IConfiguration cfg,
        GenericRepository genericRepository
    ){
        //_cs = cfg.GetConnectionString("URL_SERVER")!;
        _genericRepository = genericRepository;
    } 

    public async Task<RefreshTokenCreateResponse> CreateAsync(RefreshTokenCreateRequest request)
    {
        //using var conn = new NpgsqlConnection(_cs);
        var plain = GenerateToken(64);
        var hash = Hash(plain);

        var res = await _genericRepository.ExecuteProcedureWithOutputsAsync<(Guid Id, DateTime ExpiresAtUtc)>(
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
                }
        );

        /*var res = await conn.QuerySingleAsync<(Guid Id, DateTime ExpiresAtUtc)>(
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
        */
        return new RefreshTokenCreateResponse { 
            PlainText = plain,
            ExpiresUtc = res.ExpiresAtUtc, 
            TokenId = res.Id 
        };
    }

    public async Task<RefreshTokenGetByHashResponse?> GetByPlainAsync(string plainText)
    {
        //using var conn = new NpgsqlConnection(_cs);
        var hash = Hash(plainText);
        return await _genericRepository.GetSingleByProcedureAsync<RefreshTokenGetByHashResponse>(
            "seguridad.sp_get_refreshtoken_by_hash",
            new { TokenHash = hash }
        );
        /*return await conn.QueryFirstOrDefaultAsync<RefreshTokenGetByHashResponse>(
            "dbo.sp_RefreshToken_GetByHash",
            new { TokenHash = hash },
            commandType: CommandType.StoredProcedure);*/
    }

    public Task RevokeAllActiveAsync(Guid userId, string? ip)
    {
        using var conn = new SqlConnection(_cs);
            await conn.ExecuteAsync(
                "dbo.sp_RefreshToken_RevokeAll",
                new { UserId = userId, Ip = ip },
                commandType: CommandType.StoredProcedure);
    }

    public Task<(string newPlainText, DateTime newExpiresUtc, Guid newTokenId)> RotateAsync(Guid oldTokenId, Guid userId, string? ip, int daysToExpire = 14)
    {
        throw new NotImplementedException();
    }

    private static string GenerateToken(int bytes) => Convert.          ToBase64String(RandomNumberGenerator.GetBytes(bytes));
    private static string Hash(string s) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(s)));
}