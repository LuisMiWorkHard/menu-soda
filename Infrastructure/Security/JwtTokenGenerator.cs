using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MenuSoda.Domain.Interfaces.Security;
using MenuSoda.Domain.Users;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenGenerator : ITokenGenerator
{
    private readonly string _secret;
    private readonly int _tokenDurationMinutes;

    public JwtTokenGenerator(string secret, string tokenDuration)
    {
        _secret = secret;
        _tokenDurationMinutes = int.Parse(tokenDuration);
    }
    // Implementation of JWT token generation
    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Usunom)
            // Add other claims as needed
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "MenuSodaAPI",
            audience: "MenuSodaClients",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_tokenDurationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}