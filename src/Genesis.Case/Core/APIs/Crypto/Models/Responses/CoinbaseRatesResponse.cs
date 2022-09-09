using Newtonsoft.Json.Linq;

namespace Core.APIs.Crypto.Models.Responses;

public class CoinbaseRatesResponse
{
    public Data? Data { get; set; }
}

public class Data
{
    public JToken? Rates { get; set; }
}