using System.Security.Claims;
using GrabAndGo.Identity.API.Features.User;
using GrabAndGo.Identity.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace GrabAndGo.Identity.UnitTests;

public class UserProfileTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CompleteUserProfile.Handler _handler;

    public UserProfileTests()
    {
        _userManagerMock = MockUserManager();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _handler = new CompleteUserProfile.Handler(_userManagerMock.Object, _httpContextAccessorMock.Object);
    }

    private static Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    [Fact]
    public async Task CompleteUserProfile_WithValidRequest_ShouldReturnTrue()
    {
        // Arrange
        var userId = "user-123";
        var user = new ApplicationUser { Id = userId };
        var command = new CompleteUserProfile.Command("Ivan", "Petrenko", "+380509876543", null);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal("Ivan", user.FirstName);
        Assert.Equal("Petrenko", user.LastName);
    }

    [Fact]
    public async Task CompleteUserProfile_WhenUserNotFound_ShouldReturnFalse()
    {
        // Arrange
        var userId = "user-123";
        var command = new CompleteUserProfile.Command("Ivan", "Petrenko", "+380509876543", null);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CompleteUserProfile_WhenNoUserIdInContext_ShouldReturnFalse()
    {
        // Arrange
        var command = new CompleteUserProfile.Command("Ivan", "Petrenko", "+380509876543", null);
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal() };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
