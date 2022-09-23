using Core.Crypto.Models;
using Core.Crypto.Models.Responses;

namespace Core.Crypto.Abstractions;

public interface ICryptoProvider
{
    Task<GetExchangeRateResponse> GetExchangeRateAsync(Currency from, Currency to);
}