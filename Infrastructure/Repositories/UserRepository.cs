using MenuSoda.Domain.Models.Repositories;
using MenuSoda.Domain.Entities;
using MenuSoda.Domain.Interfaces.Repositories;
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

    public async Task<User?> GetByDocumentAsync(UsuarioGetByDocumentRequest request,  CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<User>(
            "seguridad.sp_get_usu_tipnumdoc",
            new
            {
                p_usutipdoc = request.TipoDocumento,
                p_usunumdoc = request.NumeroDocumento
            },
            ct
        );
    }

    public async Task<User?> GetByIdAsync(UsuarioGetByIdRequest request,  CancellationToken ct)
    {
        return await _genericRepository.GetSingleByProcedureAsync<User>(
            "seguridad.sp_get_usu_id",
            new
            {
                p_usuid = request.Id
            },
            ct
        );
    }

    public async Task ActualizarBloqueoAsync(User usuario, CancellationToken ct)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_id", usuario.Id, DbType.Int32);
        parameters.Add("p_usuintfall", usuario.Usuintfall, DbType.Int32);
        // Enviamos como string ISO para evitar problemas de mapeo de tipos con Npgsql
        parameters.Add("p_usufecblo", usuario.Usufecblo?.ToString("yyyy-MM-dd HH:mm:ss.ffffffZ"), DbType.String); 

        await _genericRepository.CallProcedureAsync(
            "seguridad.sp_update_usu_bloqueo",
            parameters,
            ct
        );
    }
}