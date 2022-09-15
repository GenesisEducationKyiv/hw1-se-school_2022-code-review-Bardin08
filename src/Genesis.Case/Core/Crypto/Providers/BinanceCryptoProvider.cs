using Core.Crypto.Abstractions;
using Core.Crypto.Api;
using Core.Crypto.Models;
using Core.Crypto.Models.Responses;

namespace Core.Crypto.Providers;

public interface IBinanceCryptoProvider
{
}

public class BinanceCryptoProvider : ICryptoProvider, IBinanceCryptoProvider
{
    private readonly IBinanceApi _binanceApi;

    public BinanceCryptoProvider(IBinanceApi binanceApi)
    {
        _binanceApi = binanceApi;
    }

    public async Task<CryptoProviderResponse> GetExchangeRateAsync(Currency from, Currency to)
    {
        var response = new CryptoProviderResponse {From = from, To = to, ExchangeRate = Decimal.MinusOne};

        var exchangeRateApiResponse = await _binanceApi.GetExchangeRateAsync(from, to);
        if (exchangeRateApiResponse is null)
        {
            return response;
        }

        response.ExchangeRate = exchangeRateApiResponse.Price;

        return response;
    }
}