using System.Security.Claims;
using GrabAndGo.BuildingBlocks.Responses;
using GrabAndGo.Identity.API.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GrabAndGo.Identity.API.Features.User;

public static class GetProfile
{
    public record Query : IRequest<object?>;

    public record UserProfileResponse(
        string Id,
        string Email,
        string Role,
        string? FirstName,
        string? LastName,
        string? PhoneNumber,
        string? AvatarUrl);

    public record BusinessProfileResponse(
        string Id,
        string Email,
        string Role,
        string? BusinessName,
        string? Description,
        string? Category,
        string? Phone,
        string? LogoUrl,
        string? Street,
        string? City,
        string? PostalCode,
        string? Country,
        double? Latitude,
        double? Longitude);

    public class Handler(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<Query, object?>
    {
        public async Task<object?> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return null;

            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Customer";

            if (role == "Business")
            {
                return new BusinessProfileResponse(
                    user.Id,
                    user.Email!,
                    role,
                    user.BusinessName,
                    user.BusinessDescription,
                    user.BusinessCategory,
                    user.BusinessPhone,
                    user.LogoUrl,
                    user.Street,
                    user.City,
                    user.PostalCode,
                    user.Country,
                    user.Latitude,
                    user.Longitude);
            }

            return new UserProfileResponse(
                user.Id,
                user.Email!,
                role,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.AvatarUrl);
        }
    }

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/user/profile", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new Query());
            return result != null 
                ? Results.Ok(ApiResponse<object>.SuccessResult(result)) 
                : Results.Unauthorized();
        })
        .RequireAuthorization()
        .WithName("GetProfile")
        .WithTags("Profiles")
        .Produces<ApiResponse<UserProfileResponse>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<BusinessProfileResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get current user profile";
            operation.Description = "Returns either a UserProfile or BusinessProfile depending on the user's role.";
            return operation;
        });
    }
}
