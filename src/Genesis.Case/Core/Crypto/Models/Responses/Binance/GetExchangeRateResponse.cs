namespace Core.Crypto.Models.Responses.Binance;

public class GetExchangeRateResponse
{
    public string? Symbol { get; set; }
    public decimal Price { get; set; }
}