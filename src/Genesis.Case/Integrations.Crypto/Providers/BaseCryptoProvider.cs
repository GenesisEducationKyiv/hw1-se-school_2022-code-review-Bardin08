using Integrations.Crypro.Contracts.Abstractions;
using Integrations.Crypro.Contracts.Models;
using Integrations.Crypto.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Integrations.Crypto.Providers;

public class BaseCryptoProvider : ICryptoProvider
{
    private static readonly List<Type> KnownProvidersInterfaces;

    static BaseCryptoProvider()
    {
        KnownProvidersInterfaces = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(p => (typeof(ICryptoProvider)).IsAssignableFrom(p))
            .SelectMany(t => t.GetInterfaces())
            .Where(i => i != typeof(ICryptoProvider) && i != typeof(ICryptoProviderChainSegment))
            .ToList();
    }

    private readonly ICryptoProvider? _nextProvider;

    public BaseCryptoProvider(
        ICryptoProviderFactory cryptoProviderFactory,
        IServiceProvider serviceProvider)
    {
        // Here will be returned a primary provider according to the configuration
        var defaultProvider = KnownProvidersInterfaces.FirstOrDefault(defaultValue: null);
        if (defaultProvider is null)
        {
            throw new InvalidOperationException("No crypto provider found");
        }

        var nextProvider = cryptoProviderFactory.CreateProvider()
                           ?? (ICryptoProvider) serviceProvider.GetRequiredService(defaultProvider);
        if (nextProvider.GetType() != GetType())
        {
            _nextProvider = nextProvider;
        }
        
        var usedProviderInterface = nextProvider.GetType().GetInterfaces()
            .FirstOrDefault(i => i != typeof(ICryptoProvider) && i != typeof(ICryptoProviderChainSegment));
        var toChain = KnownProvidersInterfaces
            .Where(i => i != usedProviderInterface);

        ICryptoProviderChainSegment currentSegment = ((ICryptoProviderChainSegment)_nextProvider!);
        foreach (var providerType in toChain)
        {
            var instance = (ICryptoProvider) serviceProvider.GetRequiredService(providerType);
            currentSegment.SetNextProvider(instance);
            currentSegment = ((ICryptoProviderChainSegment?) instance)!;
        }
    }

    public async Task<GetExchangeRateResponse> GetExchangeRateAsync(Currency from, Currency to)
    {
        if (_nextProvider != null)
        {
            return await _nextProvider.GetExchangeRateAsync(from, to);
        }

        return new GetExchangeRateResponse {From = from, To = to, ExchangeRate = decimal.MinusOne};
    }
}