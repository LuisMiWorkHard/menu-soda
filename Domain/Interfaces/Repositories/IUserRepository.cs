using MenuSoda.Application.Dto;
using MenuSoda.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByDocumentAsync(UsuarioGetByDocumentRequest request, CancellationToken ct);
    Task<User?> GetByIdAsync(UsuarioGetByIdRequest request, CancellationToken ct);
}