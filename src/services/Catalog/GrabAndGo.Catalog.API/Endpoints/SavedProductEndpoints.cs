using System.Security.Claims;
using GrabAndGo.BuildingBlocks.Responses;
using GrabAndGo.Catalog.Application.Commands;
using GrabAndGo.Catalog.Application.Dtos;
using GrabAndGo.Catalog.Application.Queries;
using MediatR;

namespace GrabAndGo.Catalog.API.Endpoints;

public static class SavedProductEndpoints
{
    public static void MapSavedProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products/saved")
                       .WithTags("Saved Products")
                       .RequireAuthorization();

        group.MapPost("/{productId:guid}", async (Guid productId, ClaimsPrincipal user, IMediator mediator) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

            var success = await mediator.Send(new SaveProductCommand(userId, productId));
            return success 
                ? Results.Ok(ApiResponse<bool>.SuccessResult(true, "Product saved successfully")) 
                : Results.BadRequest(ApiResponse<bool>.FailureResult("Failed to save product"));
        })
        .WithName("SaveProduct")
        .WithOpenApi();

        group.MapDelete("/{productId:guid}", async (Guid productId, ClaimsPrincipal user, IMediator mediator) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

            var success = await mediator.Send(new UnsaveProductCommand(userId, productId));
            return success 
                ? Results.Ok(ApiResponse<bool>.SuccessResult(true, "Product unsaved successfully")) 
                : Results.BadRequest(ApiResponse<bool>.FailureResult("Failed to unsave product"));
        })
        .WithName("UnsaveProduct")
        .WithOpenApi();

        group.MapGet("/", async (ClaimsPrincipal user, IMediator mediator) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

            var products = await mediator.Send(new GetSavedProductsQuery(userId));
            return Results.Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResult(products));
        })
        .WithName("GetSavedProducts")
        .WithOpenApi();
    }
}
