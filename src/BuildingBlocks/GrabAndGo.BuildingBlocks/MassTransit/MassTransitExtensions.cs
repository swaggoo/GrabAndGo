using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GrabAndGo.BuildingBlocks.MassTransit;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration, Action<IBusRegistrationConfigurator>? configure = null)
    {
        services.AddMassTransit(config =>
        {
            configure?.Invoke(config);

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

    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
    {
        return services.AddMessageBus(configuration, config =>
        {
            config.AddConsumers(assembly);
        });
    }
}
