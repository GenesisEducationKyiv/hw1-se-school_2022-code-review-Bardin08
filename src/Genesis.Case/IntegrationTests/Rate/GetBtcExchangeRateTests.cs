using System.Net.Http;
using System.Threading.Tasks;
using Api;
using Xunit;

namespace IntegrationTests.Rate;

public class GetBtcExchangeRateTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    public GetBtcExchangeRateTests(
        CustomWebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateDefaultClient();
    }

    [Fact]
    public async Task Get_BtcExchangeRate_ReturnsExchangeRate()
    {
        var response = await _httpClient.GetAsync("/rate");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotEqual(decimal.MinusOne, decimal.Parse(responseString));
    }
}