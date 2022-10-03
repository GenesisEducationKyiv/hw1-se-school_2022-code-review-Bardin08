using Integration.RabbitMq.Abstractions;
using Integration.RabbitMq.Models;
using Integration.RabbitMq.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.RabbitMq;

public static class DependencyInjection
{
    public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitConfig = new RabbitConfiguration();   
        configuration.Bind("RabbitMq", rabbitConfig);
        services.AddSingleton(rabbitConfig);

        services.AddSingleton<IProducer, Producer>();
    }
}