using System.Diagnostics;
using System.Net;
using System.Text.Json;
using GrabAndGo.BuildingBlocks.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GrabAndGo.BuildingBlocks.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var response = ApiResponse<object>.FailureResult(
            "An internal server error occurred.",
            new List<ApiError> { new ApiError { Message = exception.Message } },
            traceId
        );

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });

        return context.Response.WriteAsync(json);
    }
}
