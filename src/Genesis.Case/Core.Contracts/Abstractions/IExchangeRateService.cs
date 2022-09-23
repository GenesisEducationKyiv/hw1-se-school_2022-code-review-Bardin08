namespace Core.Contracts.Abstractions;

public interface IExchangeRateService
{
    Task<decimal> GetBtcToUahExchangeRateAsync();
}