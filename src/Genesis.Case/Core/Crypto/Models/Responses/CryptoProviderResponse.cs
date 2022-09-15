namespace Core.Crypto.Models.Responses;

public class CryptoProviderResponse
{
    public Currency From { get; set; }
    public Currency To { get; set; }
    public decimal ExchangeRate { get; set; }
}