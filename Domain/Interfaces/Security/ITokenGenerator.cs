using MenuSoda.Domain.Users;

namespace MenuSoda.Domain.Interfaces.Security
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
    }
}