using GrabAndGo.BuildingBlocks.Responses;
using GrabAndGo.Order.Application.Commands;
using GrabAndGo.Order.Application.Dtos;
using GrabAndGo.Order.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GrabAndGo.Order.API.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapGet("/", async (string userId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetOrdersQuery(userId));
            return Results.Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(result));
        })
        .RequireAuthorization()
        .Produces<ApiResponse<IEnumerable<OrderDto>>>(StatusCodes.Status200OK)
        .WithOpenApi(operation => {
            operation.Summary = "Get orders for a user or business";
            operation.Description = "Returns a list of all orders associated with the specified user ID (as customer) or business ID (as provider).";
            operation.Parameters[0].Description = "The unique identifier of the user or business.";
            return operation;
        });

        group.MapGet("/{id}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetOrderByIdQuery(id));
            return result != null 
                ? Results.Ok(ApiResponse<OrderDto>.SuccessResult(result)) 
                : Results.NotFound(ApiResponse<object>.FailureResult("Order not found"));
        })
        .WithName("GetOrderById")
        .RequireAuthorization()
        .Produces<ApiResponse<OrderDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
        .WithOpenApi(operation => {
            operation.Summary = "Get order by ID";
            operation.Description = "Returns the details of a specific order including product information.";
            return operation;
        });

        group.MapPost("/", async ([FromBody] CreateOrderCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.CreatedAtRoute("GetOrderById", new { id = result.Id }, ApiResponse<OrderDto>.SuccessResult(result));
        })
        .RequireAuthorization()
        .Produces<ApiResponse<OrderDto>>(StatusCodes.Status201Created)
        .WithOpenApi(operation => {
            operation.Summary = "Create a new order";
            operation.Description = "Places a new order for a specific product from a business.";
            
            // Add Request Example
            operation.RequestBody.Content["application/json"].Example = new OpenApiObject
            {
                ["userId"] = new OpenApiString("user-123"),
                ["businessId"] = new OpenApiString("mcdonalds-01"),
                ["productId"] = new OpenApiString("b1111111-1111-1111-1111-111111111111"),
                ["totalAmount"] = new OpenApiDouble(15.50)
            };
            
            return operation;
        });

        group.MapPatch("/status", async ([FromBody] UpdateOrderStatusCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result 
                ? Results.Ok(ApiResponse<bool>.SuccessResult(true, "Order status updated successfully")) 
                : Results.NotFound(ApiResponse<object>.FailureResult("Order not found"));
        })
        .RequireAuthorization()
        .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
        .WithOpenApi(operation => {
            operation.Summary = "Update order status";
            operation.Description = "Updates the status of an existing order (e.g., to ACCEPTED or COMPLETED).";
            
            // Add Request Example
            operation.RequestBody.Content["application/json"].Example = new OpenApiObject
            {
                ["orderId"] = new OpenApiString("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                ["status"] = new OpenApiInteger(1) // ACCEPTED
            };

            return operation;
        });
    }
}
