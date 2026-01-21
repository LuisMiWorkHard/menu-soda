using MenuSoda.Domain.Entities;

namespace MenuSoda.Domain.Interfaces.Security
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
    }
}