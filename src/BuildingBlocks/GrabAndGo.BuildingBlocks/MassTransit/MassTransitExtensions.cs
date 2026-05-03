using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GrabAndGo.BuildingBlocks.MassTransit;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
    {
        services.AddMassTransit(config =>
        {
            if (assembly != null)
            {
                config.AddConsumers(assembly);
            }

            config.UsingRabbitMq((context, b) =>
            {
                var host = configuration["RabbitMq:Host"] ?? "rabbitmq";
                b.Host(host, "/", h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? "guest");
                    h.Password(configuration["RabbitMq:Password"] ?? "guest");
                });

                b.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
