using System.Security.Claims;
using FluentValidation;
using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.BuildingBlocks.Responses;
using GrabAndGo.Identity.API.Models;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GrabAndGo.Identity.API.Features.Business;

public static class CompleteBusinessProfile
{
    public record Command(
        string BusinessName,
        string? Description,
        string? Category,
        string? Phone,
        string? LogoUrl,
        string? CoverImageUrl,
        string? Website,
        string Street,
        string City,
        string PostalCode,
        string Country,
        double Latitude,
        double Longitude) : IRequest<bool>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.BusinessName).NotEmpty();
            RuleFor(x => x.Street).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.PostalCode).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
        }
    }

    public class Handler(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IPublishEndpoint publishEndpoint)
        : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return false;

            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.BusinessName = request.BusinessName;
            user.BusinessDescription = request.Description;
            user.BusinessCategory = request.Category;
            user.BusinessPhone = request.Phone;
            user.BusinessWebsite = request.Website;
            user.LogoUrl = request.LogoUrl;
            user.CoverImageUrl = request.CoverImageUrl;
            user.Street = request.Street;
            user.City = request.City;
            user.PostalCode = request.PostalCode;
            user.Country = request.Country;
            user.Latitude = request.Latitude;
            user.Longitude = request.Longitude;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) return false;

            await publishEndpoint.Publish(new BusinessOnboardedEvent
            {
                BusinessId = user.Id,
                BusinessName = user.BusinessName,
                Description = user.BusinessDescription,
                Category = user.BusinessCategory ?? "General",
                Phone = user.BusinessPhone,
                Website = user.BusinessWebsite,
                LogoUrl = user.LogoUrl,
                CoverImageUrl = user.CoverImageUrl,
                Email = user.Email!,
                TotalOrders = user.TotalOrders,
                Street = user.Street,
                City = user.City,
                PostalCode = user.PostalCode,
                Country = user.Country,
                Latitude = user.Latitude ?? 0,
                Longitude = user.Longitude ?? 0
            }, cancellationToken);

            return true;
        }
    }

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/business/profile", async (Command command, IMediator mediator) =>
        {
            var success = await mediator.Send(command);
            return success 
                ? Results.Ok(ApiResponse<object>.SuccessResult(null, "Business profile completed successfully")) 
                : Results.BadRequest(ApiResponse<object>.FailureResult("Profile completion failed"));
        }).RequireAuthorization("BusinessOnly")
          .WithName("CompleteBusinessProfile")
          .WithTags("Profiles")
          .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
          .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
          .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
          .WithOpenApi(operation =>
          {
              operation.Summary = "Complete business profile";
              operation.Description = "Updates the business details for the authenticated user. Required for 'Business' role.";

              var example = new OpenApiObject
              {
                  ["businessName"] = new OpenApiString("Shoco Bakery"),
                  ["description"] = new OpenApiString("Modern bakery and cafe in the heart of Lviv"),
                  ["category"] = new OpenApiString("Bakery & Cafe"),
                  ["phone"] = new OpenApiString("+380 67 123 45 67"),
                  ["website"] = new OpenApiString("https://shoco.com.ua"),
                  ["logoUrl"] = new OpenApiString("https://shoco.com.ua/assets/logo.png"),
                  ["coverImageUrl"] = new OpenApiString("https://shoco.com.ua/assets/cover.jpg"),
                  ["street"] = new OpenApiString("Sakharova St, 42"),
                  ["city"] = new OpenApiString("Lviv"),
                  ["postalCode"] = new OpenApiString("79000"),
                  ["country"] = new OpenApiString("Ukraine"),
                  ["latitude"] = new OpenApiDouble(49.8286),
                  ["longitude"] = new OpenApiDouble(24.0155)
              };

              operation.RequestBody.Content["application/json"].Examples.Add("Shoco Bakery (Lviv)", new OpenApiExample { Value = example });

              return operation;
          });
    }
}
