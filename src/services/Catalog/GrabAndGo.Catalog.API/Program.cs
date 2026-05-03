using GrabAndGo.Catalog.API.Endpoints;
using GrabAndGo.Catalog.API.Extensions;
using GrabAndGo.BuildingBlocks.Middleware;
using GrabAndGo.Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCatalogServices(builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.RouteTemplate = "/openapi/{documentName}.json");
    app.UseSwaggerUI();
    app.MapScalarApiReference(options => 
    {
        options.WithTitle("GrabAndGo Products API")
               .WithTheme(ScalarTheme.DeepSpace)
               .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
               .WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

app.UseAuthentication();
app.UseAuthorization();

// Map Endpoints
app.MapProductEndpoints();
app.MapBusinessEndpoints();
app.MapCategoryEndpoints();
app.MapSavedProductEndpoints();

// Apply Migrations & Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CatalogDbContext>();
    context.Database.Migrate();
    CatalogContextSeed.SeedData(context);
}

app.Run();
