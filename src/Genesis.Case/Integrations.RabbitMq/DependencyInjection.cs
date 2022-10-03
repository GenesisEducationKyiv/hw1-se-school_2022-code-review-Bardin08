using Integrations.RabbitMq.Abstractions;
using Integrations.RabbitMq.Models;
using Integrations.RabbitMq.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Integrations.RabbitMq;

public static class DependencyInjection
{
    public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitConfig = new RabbitConfiguration();   
        configuration.Bind("RabbitMq", rabbitConfig);
        services.AddSingleton(rabbitConfig);

        services.AddSingleton<IAmqpProducer, AmqpProducer>();
    }
}