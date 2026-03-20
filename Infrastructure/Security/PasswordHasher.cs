using System.Security.Cryptography;
using MenuSoda.Application.Interfaces.Security;

namespace MenuSoda.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    const int SaltSize = 16, KeySize = 32, Iterations = 100_000;

    public string Hash(string password)
    {
        // 1. Salt aleatorio
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        // 2. Derivar clave
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(KeySize);

        // 3. Empaquetar: formato: iteraciones.saltBase64.hashBase64
        var saltB64 = Convert.ToBase64String(salt);
        var keyB64 = Convert.ToBase64String(key);
        return $"PBKDF2_SHA256:{Iterations}:{saltB64}:{keyB64}";
    }

    public bool Verify(string password, string hashedPassword)
    {
        if (hashedPassword == null) return false;
        var parts = hashedPassword.Split(':');
        if (parts.Length != 4) return false;
        var iterations = int.Parse(parts[1]);
        var salt = Convert.FromBase64String(parts[2]);
        var expectedKey = Convert.FromBase64String(parts[3]);

        // 2. Recalcular
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var actualKey = pbkdf2.GetBytes(expectedKey.Length);

        // 3. Comparar en tiempo constante
        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }
}