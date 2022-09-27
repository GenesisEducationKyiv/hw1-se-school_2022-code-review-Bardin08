using Integrations.Crypro.Contracts.Models;

namespace Integrations.Crypro.Contracts.Abstractions;

public interface ICryptoProvider
{
    Task<GetExchangeRateResponse> GetExchangeRateAsync(Currency from, Currency to);
}