using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Identity.API.Features.Auth;
using GrabAndGo.Identity.API.Models;
using GrabAndGo.Identity.API.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace GrabAndGo.Identity.UnitTests;

public class RegisterHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Register.Handler _handler;

    public RegisterHandlerTests()
    {
        _userManagerMock = MockUserManager();
        _roleManagerMock = MockRoleManager();
        _tokenServiceMock = new Mock<ITokenService>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();

        _handler = new Register.Handler(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _tokenServiceMock.Object,
            _publishEndpointMock.Object);
    }

    private static Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private static Mock<RoleManager<IdentityRole>> MockRoleManager()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        return new Mock<RoleManager<IdentityRole>>(store.Object, null!, null!, null!, null!);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateUserAndReturnToken()
    {
        // Arrange
        var command = new Register.Command("test@example.com", "Password123!", "Customer");
        var token = "mock-token";

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock.Setup(x => x.RoleExistsAsync(command.Role))
            .ReturnsAsync(true);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), command.Role))
            .ReturnsAsync(IdentityResult.Success);

        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()))
            .Returns(token);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(token, result!.Token);
        
        _userManagerMock.Verify(x => x.CreateAsync(It.Is<ApplicationUser>(u => u.Email == command.Email), command.Password), Times.Once);
        _publishEndpointMock.Verify(x => x.Publish(It.Is<UserRegisteredEvent>(e => e.Email == command.Email), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingRole_ShouldNotCreateRole()
    {
        // Arrange
        var command = new Register.Command("test@example.com", "Password123!", "Customer");

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock.Setup(x => x.RoleExistsAsync(command.Role))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _roleManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCreateFails_ShouldReturnNull()
    {
        // Arrange
        var command = new Register.Command("test@example.com", "Password123!", "Customer");

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
