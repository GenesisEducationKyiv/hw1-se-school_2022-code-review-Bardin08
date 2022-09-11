using Core.Abstractions;
using Core.APIs;
using Core.APIs.Crypto.Models;

namespace Core.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly ICoinBaseApi _coinbaseApi;

    public ExchangeRateService(ICoinBaseApi coinbaseApi)
    {
        _coinbaseApi = coinbaseApi;
    }

    public async Task<decimal> GetBtcToUahExchangeRateAsync()
    {
        var exchangeRate = await _coinbaseApi.GetExchangeRateAsync(Currency.Btc);
        if (exchangeRate is null)
        {
            return decimal.MinusOne;
        }

        var uahCurrencyCode = Currency.Uah.ToString().ToUpper();
        var btcToUah = exchangeRate!.Data!.Rates![uahCurrencyCode]!.ToString();
        var isParsed = decimal.TryParse(btcToUah, out var exchangeRateValue);
        return isParsed ? exchangeRateValue : decimal.MinusOne;
    }
}