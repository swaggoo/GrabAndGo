using GrabAndGo.Identity.API.Features.Auth;
using GrabAndGo.Identity.API.Models;
using GrabAndGo.Identity.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace GrabAndGo.Identity.UnitTests;

public class LoginHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Login.Handler _handler;

    public LoginHandlerTests()
    {
        _userManagerMock = MockUserManager();
        _signInManagerMock = MockSignInManager();
        _tokenServiceMock = new Mock<ITokenService>();

        _handler = new Login.Handler(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _tokenServiceMock.Object);
    }

    private static Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private Mock<SignInManager<ApplicationUser>> MockSignInManager()
    {
        return new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
            null!,
            null!);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var command = new Login.Command("test@example.com", "Password123!");
        var user = new ApplicationUser { Email = command.Email, Id = "1" };
        var roles = new List<string> { "Customer" };
        var token = "mock-token";

        _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);
        
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, command.Password, false))
            .ReturnsAsync(SignInResult.Success);

        _userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(roles);

        _tokenServiceMock.Setup(x => x.GenerateToken(user, roles))
            .Returns(token);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(token, result!.Token);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        var command = new Login.Command("nonexistent@example.com", "Password123!");

        _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var command = new Login.Command("test@example.com", "WrongPassword");
        var user = new ApplicationUser { Email = command.Email, Id = "1" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);
        
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, command.Password, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
