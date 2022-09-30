using Newtonsoft.Json.Linq;

namespace Integrations.Crypto.Models.ExternalResponses.CoinBase;

public class GetExchangeRateResponse
{
    public Data? Data { get; set; }
}

public class Data
{
    public JToken? Rates { get; set; }
}