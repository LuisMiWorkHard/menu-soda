using MenuSoda.Domain.Entities;

namespace MenuSoda.Application.Interfaces.Security
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
    }
}