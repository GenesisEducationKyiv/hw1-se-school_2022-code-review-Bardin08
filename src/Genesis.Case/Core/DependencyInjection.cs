using Core.Abstractions;
using Core.Contracts.Abstractions;
using Core.Crypto;
using Core.Crypto.Abstractions;
using Core.Crypto.Api.Binance;
using Core.Crypto.Api.CoinBase;
using Core.Crypto.Providers;
using Core.Services;
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

        services.AddAutoMapper(typeof(DependencyInjection));
        
        services.AddTransient<ICoinBaseApi, CoinBaseApi>();
        services.AddTransient<IBinanceApi, BinanceApi>();

        services.AddTransient<IExchangeRateService, ExchangeRateService>();
        services.AddTransient<ISubscriptionService, SubscriptionService>();
        services.AddTransient<IEmailService, EmailService>();

        services.AddHttpClient<ICoinBaseApi, CoinBaseApi>(client =>
        {
            client.BaseAddress = new Uri("https://api.coinbase.com/v2/");
        }).AddPolicyHandler(HttpRetryPolicies.GetRetryPolicy());

        services.AddHttpClient<IBinanceApi, BinanceApi>(client =>
        {
            client.BaseAddress = new Uri("https://api.binance.com/api/v3/");
        }).AddPolicyHandler(HttpRetryPolicies.GetRetryPolicy());

        services.AddTransient<ICoinBaseCryptoProvider, CoinBaseCryptoProvider>();
        services.AddTransient<IBinanceCryptoProvider, BinanceCryptoProvider>();

        services.AddTransient<ICryptoProviderFactory, CryptoProviderFactory>();
        services.AddTransient<ICryptoProvider, BaseCryptoProvider>();

        services.AddScoped<ICoinBaseApiProxy, CoinBaseApiProxy>();
        services.AddScoped<IBinanceApiProxy, BinanceApiProxy>();

        services.AddSingleton<IMemoryCache, MemoryCache>();
    }
}