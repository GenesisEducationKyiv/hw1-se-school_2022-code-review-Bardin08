using Core.Crypto.Models;
using Core.Crypto.Models.Responses.Binance;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Crypto.Api.Binance;

public interface IBinanceApiProxy : IBinanceApi
{
}

public class BinanceApiProxy : IBinanceApiProxy
{
    private readonly IBinanceApi _binanceApi;
    private readonly IMemoryCache _memoryCache;

    public BinanceApiProxy(
        IBinanceApi binanceApi,
        IMemoryCache memoryCache)
    {
        _binanceApi = binanceApi;
        _memoryCache = memoryCache;
    }

    public async Task<GetExchangeRateResponse?> GetExchangeRateAsync(Currency from, Currency to)
    {
        var cacheKey = $"binance-ex-rate-{from}-{to}";

        var isCacheReceived = _memoryCache.TryGetValue(cacheKey, out var cache);
        if (isCacheReceived && cache is GetExchangeRateResponse response)
        {
            return response;
        }

        var apiResponse = await _binanceApi.GetExchangeRateAsync(from, to);
        if (apiResponse?.Price != decimal.Zero)
        {
            _memoryCache.Set(cacheKey, apiResponse, TimeSpan.FromMinutes(5));
        }

        return apiResponse;
    }
}