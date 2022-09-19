using Core.Crypto.Abstractions;
using Core.Crypto.Api;
using Core.Crypto.Models;
using Core.Crypto.Models.Responses;

namespace Core.Crypto.Providers;

public interface IBinanceCryptoProvider
{
}

public class BinanceCryptoProvider : ICryptoProvider, IBinanceCryptoProvider, ICryptoProviderChainSegment
{
    private readonly IBinanceApi _binanceApi;

    private ICryptoProvider? _nextProvider;

    public BinanceCryptoProvider(IBinanceApi binanceApi)
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