namespace Integrations.Crypto.Models.ExternalResponses.Binance;

public class GetExchangeRateResponse
{
    public string? Symbol { get; set; }
    public decimal Price { get; set; }
}