using GrabAndGo.BuildingBlocks.Auth;
using GrabAndGo.BuildingBlocks.Middleware;
using GrabAndGo.BuildingBlocks.Responses;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.RouteTemplate = "/openapi/{documentName}.json");
    app.UseSwaggerUI();
    app.MapScalarApiReference(options => 
    {
        options.WithOpenApiRoutePattern("/openapi/v1.json")
               .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl);
    });
}

app.UseCors("AllowAll");
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/media/upload", async (IFormFile file, IWebHostEnvironment env, HttpContext context) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest(ApiResponse<object>.FailureResult("No file uploaded"));

    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

    if (!allowedExtensions.Contains(extension))
        return Results.BadRequest(ApiResponse<object>.FailureResult("Invalid file type"));

    var webRootPath = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
    var uploadsFolder = Path.Combine(webRootPath, "uploads");
    if (!Directory.Exists(uploadsFolder))
        Directory.CreateDirectory(uploadsFolder);

    var fileName = $"{Guid.NewGuid()}{extension}";
    var filePath = Path.Combine(uploadsFolder, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    var publicBaseUrl = builder.Configuration["PUBLIC_BASE_URL"]?.TrimEnd('/');
    if (string.IsNullOrEmpty(publicBaseUrl))
    {
        publicBaseUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}";
    }
    
    var fileUrl = $"{publicBaseUrl}/api/media/uploads/{fileName}";

    return Results.Ok(ApiResponse<object>.SuccessResult(new { Url = fileUrl }, "File uploaded successfully"));
})
.RequireAuthorization() // Require authentication for uploads
.DisableAntiforgery();

app.Run();
