using GrabAndGo.BuildingBlocks.Responses;
using GrabAndGo.Catalog.Application.Dtos;
using GrabAndGo.Catalog.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GrabAndGo.Catalog.API.Endpoints;

public static class BusinessEndpoints
{
    public static void MapBusinessEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products/businesses")
                       .WithTags("Businesses");

        group.MapGet("/", async ([FromQuery] string? name, [FromQuery] string? sort, [FromQuery] bool? isActive, IMediator mediator) =>
        {
            var query = new GetBusinessesQuery(name, sort, isActive);
            var businesses = await mediator.Send(query);
            return Results.Ok(ApiResponse<IEnumerable<BusinessDto>>.SuccessResult(businesses));
        })
        .WithName("GetBusinesses")
        .WithOpenApi();

        group.MapGet("/{id}", async (string id, IMediator mediator) =>
        {
            var query = new GetBusinessByIdQuery(id);
            var business = await mediator.Send(query);
            
            return business is not null 
                ? Results.Ok(ApiResponse<BusinessDto>.SuccessResult(business)) 
                : Results.NotFound(ApiResponse<object>.FailureResult("Business not found"));
        })
        .WithName("GetBusiness")
        .WithOpenApi();
    }
}
