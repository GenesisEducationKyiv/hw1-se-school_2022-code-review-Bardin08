using Core.Crypto.Abstractions;
using Core.Crypto.Api;
using Core.Crypto.Models;
using Core.Crypto.Models.Responses;

namespace Core.Crypto.Providers;

public interface ICoinBaseCryptoProvider
{
}

public class CoinBaseCryptoProvider : ICryptoProvider, ICoinBaseCryptoProvider
{
    private readonly ICoinBaseApi _coinBaseApi;

    public CoinBaseCryptoProvider(ICoinBaseApi coinBaseApi)
    {
        _coinBaseApi = coinBaseApi;
    }

    public async Task<CryptoProviderResponse> GetExchangeRateAsync(Currency from, Currency to)
    {
        var response = new CryptoProviderResponse {From = from, To = to, ExchangeRate = decimal.MinusOne};

        var exchangeRate = await _coinBaseApi.GetExchangeRateAsync(from);
        if (exchangeRate is null)
        {
            return response;
        }

        var requestedCurrencyCode = to.ToString().ToUpper();
        var btcToUah = exchangeRate.Data!.Rates![requestedCurrencyCode]!.ToString();
        var isParsed = decimal.TryParse(btcToUah, out var exchangeRateValue);
        if (isParsed)
        {
            response.ExchangeRate = exchangeRateValue;
        }

        return response;
    }
}