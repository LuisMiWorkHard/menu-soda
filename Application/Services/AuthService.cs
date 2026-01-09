using MenuSoda.Application.Dto;
using MenuSoda.Domain.Interfaces.Security;

namespace MenuSoda.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<string?> LoginAsync(LoginRequest request)
    {
        //var password = _passwordHasher.Hash(request.Contrasena);
        var user = await _userRepository.GetByDocumentAsync(new UsuarioGetByDocumentRequest
        {
            TipoDocumento = request.TipoDocumento,
            NumeroDocumento = request.NumeroDocumento
        });
        if (user == null || !_passwordHasher.Verify(request.Contrasena, user.Usuhash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        return _tokenGenerator.GenerateToken(user);
    }
}