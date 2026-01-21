using MenuSoda.Domain.Models.Repositories;
using MenuSoda.Domain.Entities;

namespace MenuSoda.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByDocumentAsync(UsuarioGetByDocumentRequest request, CancellationToken ct);
    Task<User?> GetByIdAsync(UsuarioGetByIdRequest request, CancellationToken ct);
}