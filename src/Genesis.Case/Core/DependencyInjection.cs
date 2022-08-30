using Core.Abstractions;
using Core.APIs;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DependencyInjection
{
    public static void AddCoreLogic(this IServiceCollection services)
    {
        services.AddTransient<ICoinBaseApi, CoinBaseApi>();

        services.AddTransient<IExchangeRateService, ExchangeRateService>();
        services.AddTransient<ISubscriptionService, SubscriptionService>();
        services.AddTransient<IEmailService, GmailEmailService>();
    }
}