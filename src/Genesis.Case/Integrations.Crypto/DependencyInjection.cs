using Core.Contracts.Crypto.Abstractions;
using Integrations.Crypto.ExternalApis.Binance;
using Integrations.Crypto.ExternalApis.CoinBase;
using Integrations.Crypto.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace Integrations.Crypto;

public static class DependencyInjection
{
    public static void AddCryptoIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();      

        services.AddTransient<ICoinBaseApi, CoinBaseApi>();
        services.AddTransient<IBinanceApi, BinanceApi>();

        services.AddTransient<ICoinBaseCryptoProvider, CoinBaseCryptoProvider>();
        services.AddTransient<IBinanceCryptoProvider, BinanceCryptoProvider>();

        services.AddTransient<ICryptoProviderFactory, CryptoProviderFactory>();
        services.AddTransient<ICryptoProvider, BaseCryptoProvider>();

        services.AddScoped<ICoinBaseApiProxy, CoinBaseApiProxy>();
        services.AddScoped<IBinanceApiProxy, BinanceApiProxy>();

        services.AddHttpClient<IBinanceApi, BinanceApi>(client =>
        {
            client.BaseAddress = new Uri("https://api.binance.com/api/v3/");
        }).AddPolicyHandler(HttpRetryPolicies.GetRetryPolicy());

        services.AddHttpClient<ICoinBaseApi, CoinBaseApi>(client =>
        {
            client.BaseAddress = new Uri("https://api.coinbase.com/v2/");
        }).AddPolicyHandler(HttpRetryPolicies.GetRetryPolicy());
    }
}