using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HealthChecks.UI.Client;

namespace GrabAndGo.BuildingBlocks.HealthChecks;

public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddGrabAndGoHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var builder = services.AddHealthChecks();
        
        var postgresConnectionString = configuration.GetConnectionString("Database");
        if (!string.IsNullOrEmpty(postgresConnectionString))
        {
            builder.AddNpgSql(postgresConnectionString, name: "postgres", tags: new[] { "db", "postgresql" });
        }

        var rabbitMqConnectionString = configuration.GetConnectionString("RabbitMq");
        if (!string.IsNullOrEmpty(rabbitMqConnectionString))
        {
            builder.AddRabbitMQ(rabbitMqConnectionString, name: "rabbitmq", tags: new[] { "broker", "rabbitmq" });
        }

        return builder;
    }

    public static IEndpointRouteBuilder MapGrabAndGoHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        endpoints.MapHealthChecks("/health/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });

        return endpoints;
    }
}
