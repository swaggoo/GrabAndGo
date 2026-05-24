using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Scalar.AspNetCore;
using System.Text;
using GrabAndGo.BuildingBlocks.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging("gateway");

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddObservability(builder.Configuration, "gateway");

var app = builder.Build();

var services = new[]
{
    new { Name = "Identity", Prefix = "auth", OpenApiUrl = "/api/identity/openapi/v1.json", Description = "Authentication, JWT issuance, and user management." },
    new { Name = "Catalog", Prefix = "products", OpenApiUrl = "/api/products/openapi/v1.json", Description = "Products, menus, pricing, and store management." },
    new { Name = "Order", Prefix = "orders", OpenApiUrl = "/api/orders/openapi/v1.json", Description = "Order lifecycle, payments, and fulfillment." },
    new { Name = "Media", Prefix = "media", OpenApiUrl = "/api/media/openapi/v1.json", Description = "Image uploads and asset management." },
    new { Name = "Analytics", Prefix = "analytics", OpenApiUrl = "/api/analytics/openapi/v1.json", Description = "Data insights and reporting (Post-MVP)." }
};

app.UseRouting();

// Map Scalar for each service
foreach (var service in services)
{
    app.MapScalarApiReference($"/docs/services/{service.Prefix}", options =>
    {
        options.WithTitle($"GrabAndGo - {service.Name} API")
               .WithTheme(ScalarTheme.Moon)
               .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
               .WithOpenApiRoutePattern(service.OpenApiUrl);
    });
}

// Landing page for all documentation
app.MapGet("/docs/services", () =>
{
    var html = new StringBuilder("""
        <!doctype html>
        <html lang="en">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
          <title>GrabAndGo API Portal</title>
          <style>
            :root {
              --primary: #0b57d0;
              --bg: #f8f9fa;
              --text: #1f1f1f;
              --card-bg: #ffffff;
              --border: #dadce0;
            }
            body { 
              font-family: 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; 
              margin: 0; 
              padding: 0;
              background-color: var(--bg);
              color: var(--text);
              display: flex;
              flex-direction: column;
              align-items: center;
              min-height: 100vh;
            }
            header {
              width: 100%;
              padding: 2rem 0;
              background: #fff;
              border-bottom: 1px solid var(--border);
              text-align: center;
              margin-bottom: 3rem;
            }
            h1 { margin: 0; font-weight: 400; font-size: 2.25rem; }
            .subtitle { color: #5f6368; margin-top: 0.5rem; }
            .container {
              max-width: 1000px;
              width: 90%;
              display: grid;
              grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
              gap: 1.5rem;
              padding-bottom: 4rem;
            }
            .card {
              background: var(--card-bg);
              border: 1px solid var(--border);
              border-radius: 8px;
              padding: 1.5rem;
              transition: transform 0.2s, box-shadow 0.2s;
              display: flex;
              flex-direction: column;
              text-decoration: none;
              color: inherit;
            }
            .card:hover {
              transform: translateY(-4px);
              box-shadow: 0 4px 20px rgba(0,0,0,0.08);
              border-color: var(--primary);
            }
            .card h2 { margin: 0 0 0.75rem 0; font-size: 1.25rem; color: var(--primary); }
            .card p { margin: 0 0 1.5rem 0; font-size: 0.95rem; color: #5f6368; flex-grow: 1; }
            .card .url { 
              font-family: monospace; 
              font-size: 0.8rem; 
              background: #f1f3f4; 
              padding: 0.25rem 0.5rem; 
              border-radius: 4px;
              color: #3c4043;
              align-self: flex-start;
            }
            footer {
              margin-top: auto;
              padding: 2rem;
              color: #5f6368;
              font-size: 0.85rem;
            }
          </style>
        </head>
        <body>
          <header>
            <h1>GrabAndGo API Portal</h1>
            <p class="subtitle">Unified access to all microservice documentation</p>
          </header>
          <main class="container">
        """);

    foreach (var service in services)
    {
        var docUrl = $"/docs/services/{service.Prefix}";
        html.AppendLine($"""
            <a href="{docUrl}" class="card">
              <h2>{service.Name} Service</h2>
              <p>{service.Description}</p>
              <span class="url">{service.OpenApiUrl}</span>
            </a>
            """);
    }

    html.Append("""
          </main>
          <footer>
            &copy; 2026 GrabAndGo Microservices MVP
          </footer>
        </body>
        </html>
        """);

    return Results.Content(html.ToString(), "text/html");
});

app.UseEndpoints(_ => { });

await app.UseOcelot();

app.Run();
