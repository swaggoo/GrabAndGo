using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GrabAndGo.Identity.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace GrabAndGo.Identity.API.Services;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
}

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "super_secret_key_that_is_long_enough_123!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["Jwt:ExpireDays"] ?? "7"));

        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
