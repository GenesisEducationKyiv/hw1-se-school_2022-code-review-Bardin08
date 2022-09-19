namespace Core.Crypto.Models.Responses;

public class GetExchangeRateResponse
{
    public Currency From { get; set; }
    public Currency To { get; set; }
    public decimal ExchangeRate { get; set; }
}