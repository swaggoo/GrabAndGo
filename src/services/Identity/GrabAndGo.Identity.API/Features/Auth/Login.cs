using FluentValidation;
using GrabAndGo.BuildingBlocks.Responses;
using GrabAndGo.Identity.API.Models;
using GrabAndGo.Identity.API.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GrabAndGo.Identity.API.Features.Auth;

public static class Login
{
    public record Command(string Email, string Password) : IRequest<LoginResponse?>;

    public record LoginResponse(string Token);

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class Handler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
        : IRequestHandler<Command, LoginResponse?>
    {
        public async Task<LoginResponse?> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null) return null;

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded) return null;

            var roles = await userManager.GetRolesAsync(user);
            var token = tokenService.GenerateToken(user, roles);

            return new LoginResponse(token);
        }
    }

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/login", async (Command command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result != null 
                ? Results.Ok(ApiResponse<LoginResponse>.SuccessResult(result, "Login successful")) 
                : Results.BadRequest(ApiResponse<object>.FailureResult("Invalid credentials"));
        })
        .WithName("Login")
        .WithTags("Auth")
        .Produces<ApiResponse<LoginResponse>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
        .WithOpenApi(operation =>
        {
            operation.Summary = "User login";
            operation.Description = "Authenticates a user and returns a JWT token.";

            var customerExample = new OpenApiObject
            {
                ["email"] = new OpenApiString("customer@example.com"),
                ["password"] = new OpenApiString("P@ssw0rd123")
            };

            var businessExample = new OpenApiObject
            {
                ["email"] = new OpenApiString("merchant@example.com"),
                ["password"] = new OpenApiString("P@ssw0rd123")
            };

            operation.RequestBody.Content["application/json"].Examples.Add("Customer Login", new OpenApiExample { Value = customerExample });
            operation.RequestBody.Content["application/json"].Examples.Add("Business Login", new OpenApiExample { Value = businessExample });

            return operation;
        });
    }
}
