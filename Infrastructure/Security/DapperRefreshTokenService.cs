using System.Security.Cryptography;
using System.Text;
using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Security;

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

    public async Task<RefreshTokenGetByHashResponse?> GetActiveByPlainAsync(string plainText)
    {
        //using var conn = new NpgsqlConnection(_cs);
        var hash = Hash(plainText);
        return await _genericRepository.GetSingleByProcedureAsync<RefreshTokenGetByHashResponse>(
            "seguridad.sp_get_active_refreshtoken_hash",
            new { p_hash = hash }
        );
        /*return await conn.QueryFirstOrDefaultAsync<RefreshTokenGetByHashResponse>(
            "dbo.sp_RefreshToken_GetByHash",
            new { TokenHash = hash },
            commandType: CommandType.StoredProcedure);*/
    }

    public async Task RevokeAllActiveAsync(int userId, string? ip)
    {
        await _genericRepository.ExecuteNonQueryProcedureAsync(
            "seguridad.sp_RefreshToken_RevokeAll",
            new { p_usuid = userId, p_ip = ip }
        );
    }

    public async Task<RefreshTokenRotateResponse> RotateAsync(RefreshTokenRotateRequest request)
    {
        var newPlain = GenerateToken(64);
        var newHash = Hash(newPlain);

        var res = await _genericRepository.ExecuteProcedureWithOutputsAsync<(Guid Id, DateTime ExpiresAtUtc)>(
            "seguridad.sp_rotate_refreshtoken",
            new { 
                p_oldid = request.OldTokenId, 
                p_usuid = request.UserId, 
                p_tokenhash = newHash, 
                p_ipcre = request.IpAddress, 
                p_useragent = request.UserAgent, 
                p_deviceid = request.DeviceId, 
                p_geolat = request.GeoLat, 
                p_geolon = request.GeoLon, 
                p_days_to_expire = request.DaysToExpire 
                });

        return new RefreshTokenRotateResponse{ 
            PlainText = newPlain, 
            ExpiresUtc = res.ExpiresAtUtc, 
            TokenId = res.Id 
        };
    }

    private static string GenerateToken(int bytes) => Convert.ToBase64String(RandomNumberGenerator.GetBytes(bytes));
    private static string Hash(string s) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(s)));
}