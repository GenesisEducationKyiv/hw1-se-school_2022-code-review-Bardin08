using Core.APIs.Crypto.Models;
using Core.APIs.Crypto.Models.Responses;
using Newtonsoft.Json;

namespace Core.APIs;

public interface ICoinBaseApi
{
    Task<CoinbaseRatesResponse?> GetExchangeRateAsync(Currency currency);
}

public class CoinBaseApi : ICoinBaseApi
{
    private readonly HttpClient _httpClient;

    public CoinBaseApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CoinbaseRatesResponse?> GetExchangeRateAsync(Currency currency)
    {
        const string endpointName = "exchange-rates";
        var requestUrl = $"{endpointName}?currency={currency}";

        var getExchangeRateResponse = await _httpClient.GetAsync(requestUrl);
        if (!getExchangeRateResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var responseBody = await getExchangeRateResponse.Content.ReadAsStringAsync();
        var responseModel = JsonConvert.DeserializeObject<CoinbaseRatesResponse>(responseBody);

        return responseModel?.Data?.Rates is null ? null : responseModel;
    }
}