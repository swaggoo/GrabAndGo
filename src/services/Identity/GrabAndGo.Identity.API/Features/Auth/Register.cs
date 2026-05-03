using FluentValidation;
using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.BuildingBlocks.Responses;
using GrabAndGo.Identity.API.Models;
using GrabAndGo.Identity.API.Services;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GrabAndGo.Identity.API.Features.Auth;

public static class Register
{
    public record Command(
        string Email, 
        string Password, 
        string Role) : IRequest<RegistrationResponse?>;

    public record RegistrationResponse(string Token);

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.Role).NotEmpty().Must(r => r == "Business" || r == "Customer");
        }
    }

    public class Handler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        IPublishEndpoint publishEndpoint)
        : IRequestHandler<Command, RegistrationResponse?>
    {
        public async Task<RegistrationResponse?> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded) return null;

            if (!await roleManager.RoleExistsAsync(request.Role))
            {
                await roleManager.CreateAsync(new IdentityRole(request.Role));
            }

            await userManager.AddToRoleAsync(user, request.Role);

            var roles = new[] { request.Role };
            var token = tokenService.GenerateToken(user, roles);

            // Publish generic registration event
            await publishEndpoint.Publish(new UserRegisteredEvent
            {
                UserId = user.Id,
                Email = user.Email!,
                Role = request.Role
            }, cancellationToken);

            return new RegistrationResponse(token);
        }
    }

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/register", async (Command command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result != null 
                ? Results.Ok(ApiResponse<RegistrationResponse>.SuccessResult(result, "User registered successfully. Please complete your profile.")) 
                : Results.BadRequest(ApiResponse<object>.FailureResult("Registration failed"));
        })
        .WithName("Register")
        .WithTags("Auth")
        .Produces<ApiResponse<RegistrationResponse>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
        .WithOpenApi(operation =>
        {
            operation.Summary = "Register a new user";
            operation.Description = "Creates a new user account. The 'Role' field must be either 'Customer' or 'Business'.";
            
            var customerExample = new OpenApiObject
            {
                ["email"] = new OpenApiString("ivan.petrenko@ukr.net"),
                ["password"] = new OpenApiString("P@ssw0rd123"),
                ["role"] = new OpenApiString("Customer")
            };

            var businessExample = new OpenApiObject
            {
                ["email"] = new OpenApiString("hello@shoco.com.ua"),
                ["password"] = new OpenApiString("P@ssw0rd123"),
                ["role"] = new OpenApiString("Business")
            };

            operation.RequestBody.Content["application/json"].Examples.Add("Customer (Petrenko)", new OpenApiExample { Value = customerExample });
            operation.RequestBody.Content["application/json"].Examples.Add("Business (Shoco)", new OpenApiExample { Value = businessExample });
            
            return operation;
        });
    }
}
