using System.Security.Claims;
using FluentValidation;
using GrabAndGo.BuildingBlocks.Responses;
using GrabAndGo.Identity.API.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GrabAndGo.Identity.API.Features.User;

public static class CompleteUserProfile
{
    public record Command(string FirstName, string LastName, string PhoneNumber, string? AvatarUrl) : IRequest<bool>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.PhoneNumber).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.AvatarUrl = request.AvatarUrl;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/user/profile", async (Command command, IMediator mediator) =>
        {
            var success = await mediator.Send(command);
            return success 
                ? Results.Ok(ApiResponse<object>.SuccessResult(null, "Profile updated successfully")) 
                : Results.BadRequest(ApiResponse<object>.FailureResult("Profile update failed"));
        }).RequireAuthorization()
          .WithName("CompleteUserProfile")
          .WithTags("Profiles")
          .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
          .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
          .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
          .WithOpenApi(operation =>
          {
              operation.Summary = "Complete user profile";
              operation.Description = "Updates the personal details for the authenticated user. Required for 'Customer' role.";

              var example = new OpenApiObject
              {
                  ["firstName"] = new OpenApiString("Ivan"),
                  ["lastName"] = new OpenApiString("Petrenko"),
                  ["phoneNumber"] = new OpenApiString("+380509876543"),
                  ["avatarUrl"] = new OpenApiString("https://example.com/profiles/ivan.jpg")
              };

              operation.RequestBody.Content["application/json"].Examples.Add("Ivan Petrenko (Lviv)", new OpenApiExample { Value = example });

              return operation;
          });
    }
}
