using Integrations.Crypro.Contracts.Abstractions;
using Integrations.Crypto.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Integrations.Crypto;

public interface ICryptoProviderFactory
{
    ICryptoProvider CreateProvider();
}

public class CryptoProviderFactory : ICryptoProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CryptoProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICryptoProvider CreateProvider()
    {
        var providerName = Environment.GetEnvironmentVariable("CRYPTO_CURRENCY_PROVIDER");

        ICryptoProvider provider = providerName switch
        {
            "coinbase" => (ICryptoProvider) _serviceProvider.GetRequiredService<ICoinBaseCryptoProvider>(),
            "binance" => (ICryptoProvider) _serviceProvider.GetRequiredService<IBinanceCryptoProvider>(),
            _ => null!
        };

        return provider;
    }
}