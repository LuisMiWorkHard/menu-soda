using System.Data;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using MenuSoda.Domain.Models.Security;
using MenuSoda.Domain.Interfaces.Security;

public class DapperRefreshTokenService : IRefreshToken
{
    private readonly GenericRepository _genericRepository;

    public DapperRefreshTokenService(
        GenericRepository genericRepository
    ){
        _genericRepository = genericRepository;
    } 

    public async Task<RefreshTokenCreateResponse> CreateAsync(RefreshTokenCreateRequest request, CancellationToken ct)
    {
        var plain = GenerateToken(64);
        var hash = Hash(plain);

        var parameters = new DynamicParameters();

        // IN
        parameters.Add("p_usuid", request.UserId, DbType.Int32);
        parameters.Add("p_tokenhash", hash, DbType.String);
        parameters.Add("p_ipcre", request.IpAddress, DbType.String);
        parameters.Add("p_useragent", request.UserAgent, DbType.String);
        parameters.Add("p_deviceid", request.DeviceId, DbType.String);
        parameters.Add("p_geolat", request.GeoLat, DbType.Decimal);
        parameters.Add("p_geolon", request.GeoLon, DbType.Decimal);
        parameters.Add("p_days_to_expire", request.DaysToExpire, DbType.Int32);

        // INOUT (PostgreSQL PROCEDURE needs InputOutput for Npgsql to allow named args in CALL)
        parameters.Add("p_id", dbType: DbType.Guid, direction: ParameterDirection.InputOutput);
        parameters.Add("p_fecexputc", dbType: DbType.DateTime, direction: ParameterDirection.InputOutput);
        
        await _genericRepository.CallProcedureAsync(
            "seguridad.sp_create_refreshtoken",
            parameters,
            ct
        );

        return new RefreshTokenCreateResponse { 
            PlainText = plain,
            ExpiresUtc = parameters.Get<DateTime>("p_fecexputc"), 
            TokenId = parameters.Get<Guid>("p_id")
        };
    }

    public async Task<RefreshTokenGetByHashResponse?> GetActiveByPlainAsync(string plainText, CancellationToken ct)
    {
        var hash = Hash(plainText);
        return await _genericRepository.GetSingleByProcedureAsync<RefreshTokenGetByHashResponse>(
            "seguridad.sp_get_active_refreshtoken_hash",
            new { p_hash = hash }, 
            ct
        );
    }

    public async Task<RefreshTokenGetByHashResponse?> GetByPlainAsync(string plainText, CancellationToken ct)
    {
        var hash = Hash(plainText);
        return await _genericRepository.GetSingleByProcedureAsync<RefreshTokenGetByHashResponse>(
            "seguridad.sp_get_refreshtoken_hash",
            new { p_hash = hash }, 
            ct
        );
    }

    public async Task RevokeAllActiveAsync(int userId, string? ip, CancellationToken ct)
    {
        await _genericRepository.ExecuteNonQueryProcedureAsync(
            "seguridad.sp_revoke_all_refreshtoken",
            new { p_usuid = userId, p_ip = ip },
            ct
        );
    }

    public async Task RevokeAsync(string plainText, string? ip, CancellationToken ct)
    {
        await _genericRepository.ExecuteNonQueryProcedureAsync(
            "seguridad.sp_revoke_refreshtoken",
            new { p_hash = Hash(plainText), p_ip = ip },
            ct
        );
    }

    public async Task<RefreshTokenRotateResponse> RotateAsync(RefreshTokenRotateRequest request, CancellationToken ct)
    {
        var newPlain = GenerateToken(64);
        var newHash = Hash(newPlain);

        var parameters = new DynamicParameters();

        // IN
        parameters.Add("p_oldid", request.OldTokenId, DbType.Guid);
        parameters.Add("p_usuid", request.UserId, DbType.Int32);
        parameters.Add("p_tokenhash", newHash, DbType.String);
        parameters.Add("p_ipcre", request.IpAddress, DbType.String);
        parameters.Add("p_useragent", request.UserAgent, DbType.String);
        parameters.Add("p_deviceid", request.DeviceId, DbType.String);
        parameters.Add("p_geolat", request.GeoLat, DbType.Decimal);
        parameters.Add("p_geolon", request.GeoLon, DbType.Decimal);
        parameters.Add("p_days_to_expire", request.DaysToExpire, DbType.Int32);

        parameters.Add("p_id", dbType: DbType.Guid, direction: ParameterDirection.InputOutput);
        parameters.Add("p_fecexputc", dbType: DbType.DateTime, direction: ParameterDirection.InputOutput);

        await _genericRepository.CallProcedureAsync(
            "seguridad.sp_rotate_refreshtoken",
            parameters,
            ct
        );

        return new RefreshTokenRotateResponse
        {
            PlainText = newPlain,
            TokenId = parameters.Get<Guid>("p_id"),
            ExpiresUtc = parameters.Get<DateTime>("p_fecexputc")
        };
    }

    private static string GenerateToken(int bytes) => Convert.ToBase64String(RandomNumberGenerator.GetBytes(bytes));
    private static string Hash(string s) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(s)));
}