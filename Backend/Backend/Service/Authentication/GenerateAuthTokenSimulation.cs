using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Service.Authentication;

public static class GenerateAuthTokenSimulation
{
    
    public 
        static string GenerateAuthToken(IConfiguration configuration, string role)
    {
        var secret = configuration["Jwt:Key"];
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            expires: DateTime.UtcNow.AddHours(1),
            claims: CreateRoles(role),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static IEnumerable<Claim> CreateRoles(string role)
    {
        return new List<Claim> { new Claim(ClaimTypes.Role, role) };
        
    }
}