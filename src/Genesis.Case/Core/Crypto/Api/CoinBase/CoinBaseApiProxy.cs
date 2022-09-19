using Core.Crypto.Models;
using Core.Crypto.Models.Responses.CoinBase;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Crypto.Api.CoinBase;

public interface ICoinBaseApiProxy : ICoinBaseApi
{
}

public class CoinBaseApiProxy : ICoinBaseApiProxy
{
    private readonly ICoinBaseApi _coinBaseApi;
    private readonly IMemoryCache _memoryCache;

    public CoinBaseApiProxy(
        ICoinBaseApi coinBaseApi,
        IMemoryCache memoryCache)
    {
        _coinBaseApi = coinBaseApi;
        _memoryCache = memoryCache;
    }

    public async Task<GetExchangeRateResponse?> GetExchangeRateAsync(Currency currency)
    {
        var cacheKey = $"binance-ex-rate-{currency}";

        var isCacheReceived = _memoryCache.TryGetValue(cacheKey, out var cache);
        if (isCacheReceived && cache is GetExchangeRateResponse response)
        {
            return response;
        }

        var apiResponse = await _coinBaseApi.GetExchangeRateAsync(currency);
        if (apiResponse is {Data.Rates: { }} &&
            apiResponse.Data.Rates[currency] is { } _)
        {
            _memoryCache.Set(cacheKey, apiResponse, TimeSpan.FromMinutes(5));
        }

        return apiResponse;
    }
}