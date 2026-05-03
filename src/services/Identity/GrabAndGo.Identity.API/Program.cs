using System.Text;
using FluentValidation;
using GrabAndGo.BuildingBlocks.Auth;
using GrabAndGo.BuildingBlocks.MassTransit;
using GrabAndGo.BuildingBlocks.Middleware;
using GrabAndGo.Identity.API.Behaviors;
using GrabAndGo.Identity.API.Data;
using GrabAndGo.Identity.API.Features.Auth;
using GrabAndGo.Identity.API.Features.Business;
using GrabAndGo.Identity.API.Features.User;
using GrabAndGo.Identity.API.Models;
using GrabAndGo.Identity.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// MediatR & FluentValidation
builder.Services.AddMessageBus(builder.Configuration);
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddHttpContextAccessor();

// Custom Services
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.RouteTemplate = "/openapi/{documentName}.json");
    app.UseSwaggerUI();
    app.MapScalarApiReference(options => 
    {
        options.WithTitle("GrabAndGo Identity API")
               .WithTheme(ScalarTheme.DeepSpace)
               .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
               .WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

app.UseAuthentication();
app.UseAuthorization();

// Map Vertical Slices
GetProfile.MapEndpoint(app);
Login.MapEndpoint(app);
Register.MapEndpoint(app);
CompleteBusinessProfile.MapEndpoint(app);
CompleteUserProfile.MapEndpoint(app);

// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
