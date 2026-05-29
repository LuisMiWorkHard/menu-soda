using MenuSoda.Domain.Entities;
using MenuSoda.Application.Interfaces;
using MenuSoda.Infrastructure.Persistence;
using Dapper;
using System.Data;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;
    private readonly GenericRepository _genericRepository;

    public UserRepository(
        DapperContext context,
        GenericRepository genericRepository
    ){
        _context = context;
        _genericRepository = genericRepository;
    }

    public async Task<User?> GetByDocumentAsync(int tipoDocumento, string numeroDocumento, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<User>(
            "seguridad.sp_get_usu_tipnumdoc",
            new
            {
                p_usutipdoc = tipoDocumento,
                p_usunumdoc = numeroDocumento
            },
            ct
        );
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<User>(
            "seguridad.sp_get_usu_id",
            new
            {
                p_usuid = id
            },
            ct
        );
    }

    public async Task ActualizarHashAsync(int id, string newHash, string usumod, CancellationToken ct)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_id",     id,      DbType.Int32);
        parameters.Add("p_hash",   newHash, DbType.String);
        parameters.Add("p_usumod", usumod,  DbType.String);
        await _genericRepository.CallProcedureAsync("seguridad.sp_update_usu_hash", parameters, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<User>(
            "seguridad.sp_get_usu_email",
            new { p_email = email },
            ct
        );
    }

    public async Task ActualizarBloqueoAsync(User usuario, int maxAttempts, int lockoutMinutes, CancellationToken ct)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_id", usuario.Id, DbType.Int32);
        parameters.Add("p_usuintfall", usuario.Usuintfall, DbType.Int32);
        parameters.Add("p_max_attempts", maxAttempts, DbType.Int32);
        parameters.Add("p_lockout_minutes", lockoutMinutes, DbType.Int32);
        
        await _genericRepository.CallProcedureAsync(
            "seguridad.sp_update_usu_bloqueo",
            parameters,
            ct
        );
    }
}