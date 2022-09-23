using Core.Crypto.Models;
using Core.Crypto.Models.Responses.CoinBase;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.Crypto.Api.CoinBase;

public interface ICoinBaseApiProxy : ICoinBaseApi
{
}

public class CoinBaseApiProxy : ICoinBaseApiProxy
{
    private readonly ICoinBaseApi _coinBaseApi;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CoinBaseApiProxy> _logger;

    public CoinBaseApiProxy(
        ICoinBaseApi coinBaseApi,
        IMemoryCache memoryCache,
        ILogger<CoinBaseApiProxy> logger)
    {
        _coinBaseApi = coinBaseApi;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<GetExchangeRateResponse?> GetExchangeRateAsync(Currency currency)
    {
        var currencyNormalized = currency.ToString().ToUpper();
        var cacheKey = $"binance-ex-rate-{currencyNormalized}";

        var isCacheReceived = _memoryCache.TryGetValue(cacheKey, out var cache);
        if (isCacheReceived && cache is GetExchangeRateResponse response)
        {
            _logger.LogDebug("Received a cached value for {CacheKey}. Value is {CachedValue}",
                cacheKey, JsonConvert.SerializeObject(response));
            return response;
        }

        var apiResponse = await _coinBaseApi.GetExchangeRateAsync(currency);
        _logger.LogDebug("Received an API response from {Provider}. Response is {ApiResponse}",
            nameof(CoinBaseApi), JsonConvert.SerializeObject(apiResponse));

        if (apiResponse is {Data.Rates: { }} &&
            apiResponse.Data.Rates[currencyNormalized] is { } _)
        {
            _memoryCache.Set(cacheKey, apiResponse, TimeSpan.FromMinutes(5));
        }

        return apiResponse;
    }
}