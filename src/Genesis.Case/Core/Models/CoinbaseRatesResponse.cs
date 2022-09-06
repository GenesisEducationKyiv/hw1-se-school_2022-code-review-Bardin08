using Newtonsoft.Json.Linq;

namespace Core.Models;

public class CoinbaseRatesResponse
{
    public Data? Data { get; set; }
}

public class Data
{
    public JToken? Rates { get; set; }
}