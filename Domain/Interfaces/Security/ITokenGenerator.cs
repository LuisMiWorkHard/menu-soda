using MenuSoda.Domain.Users;

public interface ITokenGenerator
{
    string GenerateToken(User user);
}