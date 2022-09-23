using System.Threading.Tasks;
using Core.Crypto.Abstractions;
using Core.Crypto.Api;
using Core.Crypto.Api.Binance;
using Core.Crypto.Models;
using Core.Crypto.Models.Responses.Binance;
using Core.Crypto.Providers;
using Moq;
using Xunit;

namespace UnitTests.Crypto.Providers;

public class BinanceCryptoProviderTests
{
    private readonly ICryptoProvider _sut;
    private readonly Mock<IBinanceApiProxy> _binanceApiMock = new();

    public BinanceCryptoProviderTests()
    {
        _sut = new BinanceCryptoProvider(_binanceApiMock.Object);
    }
    
    [Fact]
    public async Task GetExchangeRateAsync_Binance_ReturnsExpectedValue()
    {
        // Arrange
        var apiResponse = new GetExchangeRateResponse()
        {
            Symbol = "BTCUAH", Price = decimal.One
        };
        _binanceApiMock.Setup(x => x.GetExchangeRateAsync(It.IsAny<Currency>(), It.IsAny<Currency>()))
            .ReturnsAsync(apiResponse);

        // Act
        var getExchangeRateResponse = await _sut.GetExchangeRateAsync(Currency.Btc, Currency.Uah);

        // Assert
        Assert.Equal(decimal.One, getExchangeRateResponse.ExchangeRate);
    }

    [Fact]
    public async Task GetExchangeRateAsync_Binance_ReturnsError()
    {
        // Arrange
        _binanceApiMock.Setup(x => x.GetExchangeRateAsync(It.IsAny<Currency>(), It.IsAny<Currency>()))
            .ReturnsAsync((GetExchangeRateResponse?)null);

        // Act
        var getExchangeRateResponse = await _sut.GetExchangeRateAsync(Currency.Btc, Currency.Uah);

        // Assert
        Assert.Equal(decimal.MinusOne, getExchangeRateResponse.ExchangeRate);
    }
}