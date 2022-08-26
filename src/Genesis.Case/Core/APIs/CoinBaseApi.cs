using Core.Models;
using Newtonsoft.Json;

namespace Core.APIs;

public interface ICoinBaseApi
{
    Task<CoinbaseRatesResponse?> GetExchangeRateAsync(Currency currency);
}

public class CoinBaseApi : ICoinBaseApi
{
    private const string BaseApi = "https://api.coinbase.com/v2/";

    public async Task<CoinbaseRatesResponse?> GetExchangeRateAsync(Currency currency)
    {
        const string endpointName = "exchange-rates";
        var requestUrl = $"{BaseApi}{endpointName}?currency={currency}";

        using var httpClient = new HttpClient();
        var getExchangeRateResponse = await httpClient.GetAsync(requestUrl);
        if (!getExchangeRateResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var responseBody = await getExchangeRateResponse.Content.ReadAsStringAsync();
        var responseModel = JsonConvert.DeserializeObject<CoinbaseRatesResponse>(responseBody);

        return responseModel?.Data!.Rates is null
            ? null
            : responseModel;
    }
}