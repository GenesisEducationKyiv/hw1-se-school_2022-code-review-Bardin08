namespace Core.Abstractions;

public interface IExchangeRateService
{
    Task<decimal> GetBtcToUahExchangeRateAsync();
}