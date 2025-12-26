using MenuSoda.Domain.Users;
using MenuSoda.Infrastructure.Persistence;

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

    public async Task<User?> GetByDocumentAsync(UsuarioGetByDocumentRequest request)
    {
        return await _genericRepository.GetSingleByProcedureAsync<User>(
            "seguridad.sp_get_usu_tipnumdoc",
            new
            {
                p_usutipdoc = request.TipoDocumento,
                p_usunumdoc = request.NumeroDocumento
            }
        );
    }
}