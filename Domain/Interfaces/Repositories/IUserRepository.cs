using MenuSoda.Application.Dto;
using MenuSoda.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByDocumentAsync(UsuarioGetByDocumentRequest request);   // Define methods for user repository here
    
}