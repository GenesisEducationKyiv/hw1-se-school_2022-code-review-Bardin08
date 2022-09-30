using Core.Contracts.Abstractions;
using Core.Contracts.Crypto.Abstractions;
using Core.Contracts.Crypto.Models;

namespace Core.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly ICryptoProvider _cryptoProvider;


    public ExchangeRateService(ICryptoProvider cryptoProvider)
    {
        _cryptoProvider = cryptoProvider;
    }

    public async Task<decimal> GetBtcToUahExchangeRateAsync()
    {
        var getExchangeRateResponse = await _cryptoProvider.GetExchangeRateAsync(Currency.Btc, Currency.Uah);
        return getExchangeRateResponse.ExchangeRate;
    }
}