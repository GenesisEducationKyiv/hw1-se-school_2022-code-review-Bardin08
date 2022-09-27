using Integrations.Crypro.Contracts.Models;
using Newtonsoft.Json;
using GetExchangeRateResponse = Integrations.Crypto.Models.ExternalResponses.CoinBase.GetExchangeRateResponse;

namespace Integrations.Crypto.ExternalApis.CoinBase;

public interface ICoinBaseApi
{
    Task<GetExchangeRateResponse?> GetExchangeRateAsync(Currency currency);
}

public class CoinBaseApi : ICoinBaseApi
{
    private readonly HttpClient _httpClient;

    public CoinBaseApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GetExchangeRateResponse?> GetExchangeRateAsync(Currency currency)
    {
        const string endpointName = "exchange-rates";
        var requestUrl = $"{endpointName}?currency={currency}";

        var getExchangeRateResponse = await _httpClient.GetAsync(requestUrl);
        if (!getExchangeRateResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var responseBody = await getExchangeRateResponse.Content.ReadAsStringAsync();
        var responseModel = JsonConvert.DeserializeObject<GetExchangeRateResponse>(responseBody);

        return responseModel?.Data?.Rates is null ? null : responseModel;
    }
}