using Core.Crypto.Models;
using Core.Crypto.Models.Responses.Binance;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Core.Crypto.Api.Binance;

public interface IBinanceApiProxy : IBinanceApi
{
}

public class BinanceApiProxy : IBinanceApiProxy
{
    private readonly IBinanceApi _binanceApi;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<BinanceApiProxy> _logger;

    public BinanceApiProxy(
        IBinanceApi binanceApi,
        IMemoryCache memoryCache,
        ILogger<BinanceApiProxy> logger)
    {
        _binanceApi = binanceApi;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<GetExchangeRateResponse?> GetExchangeRateAsync(Currency from, Currency to)
    {
        var cacheKey = $"binance-ex-rate-{from}-{to}";

        var isCacheReceived = _memoryCache.TryGetValue(cacheKey, out var cache);
        if (isCacheReceived && cache is GetExchangeRateResponse response)
        {
            _logger.LogDebug("Received a cached value for {CacheKey}. Value is {CachedValue}",
                cacheKey, JsonConvert.SerializeObject(response));

            return response;
        }

        var apiResponse = await _binanceApi.GetExchangeRateAsync(from, to);
        _logger.LogDebug("Received an API response from {Provider}. Response is {ApiResponse}",
            nameof(BinanceApi), JsonConvert.SerializeObject(apiResponse));

        if (apiResponse?.Price != decimal.Zero)
        {
            _memoryCache.Set(cacheKey, apiResponse, TimeSpan.FromMinutes(5));
        }

        return apiResponse;
    }
}