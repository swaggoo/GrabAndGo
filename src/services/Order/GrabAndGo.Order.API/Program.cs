using GrabAndGo.Order.API.Endpoints;
using GrabAndGo.Order.API.Extensions;
using GrabAndGo.BuildingBlocks.Middleware;
using GrabAndGo.Order.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOrderServices(builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.RouteTemplate = "/openapi/{documentName}.json");
    app.UseSwaggerUI();
    app.MapScalarApiReference(options => 
    {
        options.WithTitle("GrabAndGo Order API")
               .WithTheme(ScalarTheme.DeepSpace)
               .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
               .WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

app.UseAuthentication();
app.UseAuthorization();

// Map Endpoints
app.MapOrderEndpoints();

// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<OrderDbContext>();
    context.Database.Migrate();
    OrderContextSeed.SeedData(context);
}

app.Run();
