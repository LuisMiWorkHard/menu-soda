using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MenuSoda.Domain.Users;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenGenerator : ITokenGenerator
{
    private readonly string _secret;

    public JwtTokenGenerator(string secret)
    {
        _secret = secret;
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
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}