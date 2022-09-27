using Integrations.Crypro.Contracts.Models;

namespace Integrations.Crypto.Models;

public class GetExchangeRateSummary
{
    public Currency From { get; set; }
    public Currency To { get; set; }
    public decimal ExchangeRate { get; set; }
}