using System.Text;
using GrabAndGo.BuildingBlocks.Auth;
using GrabAndGo.BuildingBlocks.MassTransit;
using GrabAndGo.Catalog.Application.Queries;
using GrabAndGo.Catalog.Application.Consumers;
using GrabAndGo.Catalog.Infrastructure.Data;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GrabAndGo.Catalog.API.Extensions;

public static class HostingExtensions
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddHttpContextAccessor();

        // Database
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Authentication & Authorization
        services.AddJwtAuthentication(configuration);

        // Message Bus
        services.AddMessageBus(configuration, typeof(BusinessOnboardedConsumer).Assembly);

        // Infrastructure
        services.AddScoped<IBusinessRepository, BusinessRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Application
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllProductsQuery).Assembly));

        return services;
    }
}
