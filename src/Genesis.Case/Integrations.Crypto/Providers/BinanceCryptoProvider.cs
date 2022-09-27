using Integrations.Crypro.Contracts.Abstractions;
using Integrations.Crypro.Contracts.Models;
using Integrations.Crypto.ExternalApis.Binance;
using ICryptoProviderChainSegment = Integrations.Crypto.Abstractions.ICryptoProviderChainSegment;

namespace Integrations.Crypto.Providers;

public interface IBinanceCryptoProvider
{
}

public class BinanceCryptoProvider : IBinanceCryptoProvider, ICryptoProvider, ICryptoProviderChainSegment
{
    private readonly IBinanceApiProxy _binanceApi;

    private ICryptoProvider? _nextProvider;

    public BinanceCryptoProvider(IBinanceApiProxy binanceApi)
    {
        _binanceApi = binanceApi;
    }

    public ICryptoProvider SetNextProvider(ICryptoProvider nextProvider)
    {
        _nextProvider = nextProvider;
        return nextProvider;
    }

    public async Task<GetExchangeRateResponse> GetExchangeRateAsync(Currency from, Currency to)
    {
        var response = new GetExchangeRateResponse {From = from, To = to, ExchangeRate = decimal.MinusOne};

        var exchangeRateApiResponse = await _binanceApi.GetExchangeRateAsync(from, to);
        if (exchangeRateApiResponse is null)
        {
            return response;
        }

        response.ExchangeRate = exchangeRateApiResponse.Price;

        if (response.ExchangeRate != decimal.MinusOne || _nextProvider == null)
        {
            return response;
        }

        var exRate = await _nextProvider.GetExchangeRateAsync(from, to);
        if (exRate.ExchangeRate != decimal.MinusOne)
        {
            response.ExchangeRate = exRate.ExchangeRate;
        }

        return response;
    }
}