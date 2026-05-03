using GrabAndGo.BuildingBlocks.Responses;
using GrabAndGo.Catalog.Application.Dtos;
using GrabAndGo.Catalog.Application.Queries;
using MediatR;

namespace GrabAndGo.Catalog.API.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products/categories")
                       .WithTags("Categories");

        group.MapGet("/", GetCategories)
             .WithName("GetCategories")
             .Produces<ApiResponse<IEnumerable<CategoryDto>>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> GetCategories(IMediator mediator)
    {
        var query = new GetCategoriesQuery();
        var categories = await mediator.Send(query);
        return Results.Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(categories));
    }
}
