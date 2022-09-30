using Core.Contracts.Crypto.Models;

namespace Core.Contracts.Crypto.Abstractions;

public interface ICryptoProvider
{
    Task<GetExchangeRateResponse> GetExchangeRateAsync(Currency from, Currency to);
}