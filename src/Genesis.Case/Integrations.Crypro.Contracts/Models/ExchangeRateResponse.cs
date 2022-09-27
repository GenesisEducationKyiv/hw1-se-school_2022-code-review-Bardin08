namespace Integrations.Crypro.Contracts.Models;

public class GetExchangeRateResponse
{
    public Currency From { get; set; }
    public Currency To { get; set; }
    public decimal ExchangeRate { get; set; }
}