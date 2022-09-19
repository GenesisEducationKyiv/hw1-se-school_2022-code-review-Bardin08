using Core.Crypto.Models;
using Core.Crypto.Models.Responses;
using Core.Crypto.Models.Responses.Binance;
using Newtonsoft.Json;
using GetExchangeRateResponse = Core.Crypto.Models.Responses.Binance.GetExchangeRateResponse;

namespace Core.Crypto.Api;

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