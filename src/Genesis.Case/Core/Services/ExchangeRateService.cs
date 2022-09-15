using Core.Abstractions;
using Core.Crypto;
using Core.Crypto.Models;

namespace Core.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly ICryptoProviderFactory _cryptoProviderFactory;

    public ExchangeRateService(ICryptoProviderFactory cryptoProviderFactory)
    {
        _cryptoProviderFactory = cryptoProviderFactory;
    }

    public async Task<decimal> GetBtcToUahExchangeRateAsync()
    {
        var provider = _cryptoProviderFactory.CreateProvider();
        var getExchangeRateResponse = await provider.GetExchangeRateAsync(Currency.Btc, Currency.Uah);
        return getExchangeRateResponse.ExchangeRate;
    }
}