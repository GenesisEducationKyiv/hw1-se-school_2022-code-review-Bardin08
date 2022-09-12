using System;
using System.Threading.Tasks;
using Core.APIs;
using Core.APIs.Crypto.Models;
using Core.APIs.Crypto.Models.Responses;
using Core.Services;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace UnitTests.Services;

public class ExchangeRateServiceTests
{
    private readonly ExchangeRateService _sut;
    private readonly Mock<ICoinBaseApi> _coinBaseApiMock = new();

    public ExchangeRateServiceTests()
    {
        _sut = new ExchangeRateService(_coinBaseApiMock.Object);
    }

    [Fact]
    public async Task GetExchangeRateAsync_ReturnsExpectedValue()
    {
        // Arrange
        var expectedValue = decimal.Parse("1.00");
        var coinBaseResponse = new CoinbaseRatesResponse
        {
            Data = new() {Rates = JToken.Parse("{\"UAH\":1.00}")}
        };
        _coinBaseApiMock.Setup(x => x.GetExchangeRateAsync(It.IsAny<Currency>()))
            .ReturnsAsync(coinBaseResponse);

        // Act
        var exchangeRate = await _sut.GetBtcToUahExchangeRateAsync();

        // Assert
        Assert.Equal(expectedValue, exchangeRate);
    }

    [Fact]
    public async Task GetExchangeRateAsync_ReturnsError()
    {
        // Arrange
        _coinBaseApiMock.Setup(x => x.GetExchangeRateAsync(It.IsAny<Currency>()))
            .ReturnsAsync((CoinbaseRatesResponse?)null);

        // Act
        var response = await _sut.GetBtcToUahExchangeRateAsync();

        // Assert
        Assert.Equal(Decimal.MinusOne, response);
    }
}