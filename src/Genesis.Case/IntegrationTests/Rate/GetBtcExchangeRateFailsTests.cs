using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Api;
using Core.APIs;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using Xunit;

namespace IntegrationTests.Rate;

public class GetBtcExchangeRateFailsTests
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    public GetBtcExchangeRateFailsTests(
        CustomWebApplicationFactory<Program> factory)
    {
        _httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

                var response = new HttpResponseMessage()
                {
                    Content = JsonContent.Create(new {error = "Invalid currency code"})
                };

                httpMessageHandlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(response);

                services.AddHttpClient<ICoinBaseApi, CoinBaseApi>(client =>
                {
                    client.BaseAddress = new Uri("https://api.coinbase/com/v2/");
                }).ConfigurePrimaryHttpMessageHandler(() => httpMessageHandlerMock.Object);
            });
        }).CreateClient();
    }
    
    [Fact]
    public async Task Get_BtcExchangeRate_ReturnsMinusOne()
    {
        var response = await _httpClient.GetAsync("/rate");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal(decimal.MinusOne, decimal.Parse(responseString));
    }
}