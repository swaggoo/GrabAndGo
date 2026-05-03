using GrabAndGo.BuildingBlocks.Responses;
using GrabAndGo.Catalog.Application.Commands;
using GrabAndGo.Catalog.Application.Dtos;
using GrabAndGo.Catalog.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace GrabAndGo.Catalog.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
                       .WithTags("Products");

        group.MapGet("/", GetAllProducts)
             .WithName("GetProducts")
             .Produces<ApiResponse<IEnumerable<ProductDto>>>(StatusCodes.Status200OK)
             .WithOpenApi(operation =>
             {
                 operation.Summary = "Get all products";
                 operation.Description = "Retrieves products with support for filtering by business, category, status, and proximity location.";
                 
                 var p = operation.Parameters;
                 p[0].Description = "Filter by Business ID (e.g. 602d2149e773f2a3990b47e0)";
                 p[1].Description = "Filter by Category ID";
                 p[2].Description = "Search by product name";
                 p[3].Description = "Sort results: 'price', '-price', 'name', '-name', or 'distance' (requires lat/long)";
                 p[4].Description = "Filter by IsActive status (true/false)";
                 p[5].Description = "Latitude for distance filtering (e.g. 49.8397)";
                 p[6].Description = "Longitude for distance filtering (e.g. 24.0297)";
                 p[7].Description = "Radius in kilometers for distance filtering (e.g. 5.0)";
                 
                 return operation;
             });

        group.MapGet("/{id:length(36)}", GetProductById)
             .WithName("GetProduct")
             .Produces<ApiResponse<ProductDto>>(StatusCodes.Status200OK)
             .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateProduct)
             .WithName("CreateProduct")
             .Produces<ApiResponse<Domain.Entities.Product>>(StatusCodes.Status201Created)
             .RequireAuthorization("BusinessOnly");

        group.MapPut("/", UpdateProduct)
             .WithName("UpdateProduct")
             .Produces<ApiResponse<bool>>()
             .RequireAuthorization("BusinessOnly");

        group.MapDelete("/{id:length(36)}", DeleteProduct)
             .WithName("DeleteProduct")
             .Produces<ApiResponse<bool>>()
             .RequireAuthorization("BusinessOnly");
    }

    private static async Task<IResult> GetAllProducts(
        [FromQuery] string? businessId, 
        [FromQuery] string? categoryId, 
        [FromQuery] string? name, 
        [FromQuery] string? sort, 
        [FromQuery] bool? isActive,
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        [FromQuery] double? radius,
        IMediator mediator)
    {
        var query = new GetAllProductsQuery(businessId, categoryId, name, sort, isActive, latitude, longitude, radius);
        var products = await mediator.Send(query);
        return Results.Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResult(products));
    }

    private static async Task<IResult> GetProductById(string id, IMediator mediator)
    {
        var query = new GetProductByIdQuery(id);
        var product = await mediator.Send(query);
        return product is not null 
            ? Results.Ok(ApiResponse<ProductDto>.SuccessResult(product)) 
            : Results.NotFound(ApiResponse<object>.FailureResult("Product not found"));
    }

    private static async Task<IResult> CreateProduct([FromBody] CreateProductCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command);
        return Results.CreatedAtRoute("GetProduct", new { id = result.Id }, ApiResponse<Domain.Entities.Product>.SuccessResult(result));
    }

    private static async Task<IResult> UpdateProduct([FromBody] UpdateProductCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command);
        return Results.Ok(ApiResponse<bool>.SuccessResult(result, "Product updated successfully"));
    }

    private static async Task<IResult> DeleteProduct(string id, IMediator mediator)
    {
        var command = new DeleteProductCommand(id);
        var result = await mediator.Send(command);
        return Results.Ok(ApiResponse<bool>.SuccessResult(result, "Product deleted successfully"));
    }
}
