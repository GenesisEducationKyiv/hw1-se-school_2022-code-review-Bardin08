using Core.Abstractions;
using Core.Contracts.Abstractions;
using Core.Services;
using Integrations.Crypto;
using Integrations.Notifications;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DependencyInjection
{
    public static void AddCoreLogic(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNotificationsIntegration(configuration);
        services.AddCryptoIntegration(configuration);
        
        
        services.AddAutoMapper(typeof(DependencyInjection));


        services.AddTransient<IExchangeRateService, ExchangeRateService>();
        services.AddTransient<ISubscriptionService, SubscriptionService>();
        services.AddTransient<IEmailService, EmailService>();
        



        services.AddSingleton<IMemoryCache, MemoryCache>();
    }
}