using Core.Contracts.Crypto.Models;
using Newtonsoft.Json;
using GetExchangeRateResponse = Integrations.Crypto.Models.ExternalResponses.Binance.GetExchangeRateResponse;

namespace Integrations.Crypto.ExternalApis.Binance;

public interface IBinanceApi
{
    Task<GetExchangeRateResponse?> GetExchangeRateAsync(Currency from, Currency to);
}

public class BinanceApi : IBinanceApi
{
    private readonly HttpClient _httpClient;

    public BinanceApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GetExchangeRateResponse?> GetExchangeRateAsync(Currency from, Currency to)
    {
        var symbol = $"{from.ToString().ToUpper()}{to.ToString().ToUpper()}";
        var requestUrl = $"ticker/price?symbol={symbol}";

        var response = await _httpClient.GetAsync(requestUrl);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();        
        var responseModel = JsonConvert.DeserializeObject<GetExchangeRateResponse>(content);

        return responseModel;
    }
}