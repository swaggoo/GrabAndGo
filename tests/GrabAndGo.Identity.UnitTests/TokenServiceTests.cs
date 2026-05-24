using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GrabAndGo.Identity.API.Models;
using GrabAndGo.Identity.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace GrabAndGo.Identity.UnitTests;

public class TokenServiceTests
{
    [Fact]
    public void GenerateToken_ShouldReturnValidToken()
    {
        // Arrange
        var myConfiguration = new Dictionary<string, string?>
        {
            {"Jwt:Key", "super_secret_key_that_is_long_enough_123!"},
            {"Jwt:Issuer", "GrabAndGo"},
            {"Jwt:Audience", "GrabAndGoUsers"},
            {"Jwt:ExpireDays", "7"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();

        // Verify configuration
        if (configuration["Jwt:Issuer"] != "GrabAndGo") {
            throw new Exception($"Config Issuer is '{configuration["Jwt:Issuer"]}'");
        }

        var tokenService = new TokenService(configuration);

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "test@example.com"
        };
        var roles = new List<string> { "Customer", "Admin" };

        // Act
        var tokenString = tokenService.GenerateToken(user, roles);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(tokenString));

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenString);

        Assert.NotNull(token);
    }
}
