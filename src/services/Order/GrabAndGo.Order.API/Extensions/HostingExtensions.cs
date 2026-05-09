using GrabAndGo.BuildingBlocks.Auth;
using GrabAndGo.Order.Application.Queries;
using GrabAndGo.Order.Domain.Repositories;
using GrabAndGo.Order.Infrastructure.Data;
using GrabAndGo.Order.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GrabAndGo.Order.API.Extensions;

public static class HostingExtensions
{
    public static IServiceCollection AddOrderServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddHttpContextAccessor();

        // Database
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Authentication & Authorization
        services.AddJwtAuthentication(configuration);

        // Infrastructure
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Application
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetOrdersQuery).Assembly));

        return services;
    }
}
