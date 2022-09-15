using Newtonsoft.Json.Linq;

namespace Core.Crypto.Models.Responses.CoinBase;

public class GetExchangeRateResponse
{
    public Data? Data { get; set; }
}

public class Data
{
    public JToken? Rates { get; set; }
}