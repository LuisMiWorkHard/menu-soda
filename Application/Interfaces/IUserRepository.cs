using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByDocumentAsync(int tipoDocumento, string numeroDocumento, CancellationToken ct);
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    Task ActualizarBloqueoAsync(User usuario, int maxAttempts, int lockoutMinutes, CancellationToken ct);
}