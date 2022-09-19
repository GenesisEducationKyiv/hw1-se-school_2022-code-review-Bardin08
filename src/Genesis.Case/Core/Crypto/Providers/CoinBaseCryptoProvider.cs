using Core.Crypto.Abstractions;
using Core.Crypto.Api;
using Core.Crypto.Models;
using Core.Crypto.Models.Responses;

namespace Core.Crypto.Providers;

public interface ICoinBaseCryptoProvider
{
}

public class CoinBaseCryptoProvider : ICryptoProvider, ICoinBaseCryptoProvider, ICryptoProviderChainSegment
{
    private readonly ICoinBaseApi _coinBaseApi;

    private ICryptoProvider? _nextProvider;

    public CoinBaseCryptoProvider(ICoinBaseApi coinBaseApi)
    {
        _coinBaseApi = coinBaseApi;
    }

    public ICryptoProvider SetNextProvider(ICryptoProvider nextProvider)
    {
        _nextProvider = nextProvider;
        return nextProvider;
    }

    public async Task<GetExchangeRateResponse> GetExchangeRateAsync(Currency from, Currency to)
    {
        var response = new GetExchangeRateResponse {From = from, To = to, ExchangeRate = decimal.MinusOne};

        var exchangeRate = await _coinBaseApi.GetExchangeRateAsync(from);
        if (exchangeRate is null)
        {
            return response;
        }

        var requestedCurrencyCode = to.ToString().ToUpper();
        var btcToUah = exchangeRate.Data!.Rates![requestedCurrencyCode]!.ToString();
        var isParsed = decimal.TryParse(btcToUah, out var exchangeRateValue);
        if (!isParsed)
        {
            if (_nextProvider == null)
            {
                return response;
            }

            var exchangeRateFromNextProvider = await _nextProvider.GetExchangeRateAsync(from, to);

            if (exchangeRateFromNextProvider.ExchangeRate != decimal.MinusOne)
            {
                response.ExchangeRate = exchangeRateFromNextProvider.ExchangeRate;
            }

            return response;
        }

        response.ExchangeRate = exchangeRateValue;

        return response;
    }
}