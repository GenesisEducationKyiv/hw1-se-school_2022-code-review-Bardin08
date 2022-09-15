using System;
using System.Threading.Tasks;
using Core.Crypto;
using Core.Crypto.Abstractions;
using Core.Crypto.Api;
using Core.Crypto.Models;
using Core.Crypto.Models.Responses;
using Core.Crypto.Models.Responses.Binance;
using Core.Crypto.Providers;
using Core.Services;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.VisualStudio.TestPlatform.Common;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;
using GetExchangeRateResponse = Core.Crypto.Models.Responses.CoinBase.GetExchangeRateResponse;

namespace UnitTests.Services;

public class ExchangeRateServiceTests
{
    private readonly ExchangeRateService _sut;
    private readonly Mock<ICryptoProviderFactory> _cryptoProviderFactoryMock = new();
    private readonly Mock<ICoinBaseApi> _coinBaseApiMock = new();
    private readonly Mock<IBinanceApi> _binanceApiMock = new();

    public ExchangeRateServiceTests()
    {
        _sut = new ExchangeRateService(_cryptoProviderFactoryMock.Object);
    }

    [Fact]
    public async Task GetExchangeRateAsync_CoinBase_ReturnsExpectedValue()
    {
        // Arrange
        var expectedValue = decimal.Parse("1.00");
        var coinBaseResponse = new GetExchangeRateResponse
        {
            Data = new Core.Crypto.Models.Responses.CoinBase.Data
            {
                Rates = JToken.Parse("{\"UAH\":1.00}")
            }
        };
        _coinBaseApiMock.Setup(x => x.GetExchangeRateAsync(Currency.Btc))
            .ReturnsAsync(coinBaseResponse);
        _cryptoProviderFactoryMock.Setup(x => x.CreateProvider())
            .Returns(new CoinBaseCryptoProvider(_coinBaseApiMock.Object));

        // Act
        var exchangeRate = await _sut.GetBtcToUahExchangeRateAsync();

        // Assert
        Assert.Equal(expectedValue, exchangeRate);
    }

    [Fact]
    public async Task GetExchangeRateAsync_CoinBase_ReturnsError()
    {
        // Arrange
        _coinBaseApiMock.Setup(x => x.GetExchangeRateAsync(It.IsAny<Currency>()))
            .ReturnsAsync((GetExchangeRateResponse?)null);
        _cryptoProviderFactoryMock.Setup(x => x.CreateProvider())
            .Returns(new CoinBaseCryptoProvider(_coinBaseApiMock.Object));
        
        // Act
        var response = await _sut.GetBtcToUahExchangeRateAsync();

        // Assert
        Assert.Equal(Decimal.MinusOne, response);
    }

    [Fact]
    public async Task GetExchangeRateAsync_Binance_ReturnsExpectedValue()
    {
        // Arrange
        var expectedValue = decimal.One;
        var apiResponse = new Core.Crypto.Models.Responses.Binance.GetExchangeRateResponse()
        {
            Symbol = "BTCUAH",
            Price = decimal.One
        };
        _binanceApiMock.Setup(x => x.GetExchangeRateAsync(Currency.Btc, Currency.Uah))
            .ReturnsAsync(apiResponse);
        _cryptoProviderFactoryMock.Setup(x => x.CreateProvider())
            .Returns(new BinanceCryptoProvider(_binanceApiMock.Object));

        // Act
        var exchangeRate = await _sut.GetBtcToUahExchangeRateAsync();

        // Assert
        Assert.Equal(expectedValue, exchangeRate);
    }

    [Fact]
    public async Task GetExchangeRateAsync_Binance_ReturnsError()
    {
        // Arrange
        _binanceApiMock.Setup(x => x.GetExchangeRateAsync(It.IsAny<Currency>(), It.IsAny<Currency>()))
            .ReturnsAsync((Core.Crypto.Models.Responses.Binance.GetExchangeRateResponse?)null);
        _cryptoProviderFactoryMock.Setup(x => x.CreateProvider())
            .Returns(new BinanceCryptoProvider(_binanceApiMock.Object));
        
        // Act
        var response = await _sut.GetBtcToUahExchangeRateAsync();

        // Assert
        Assert.Equal(Decimal.MinusOne, response);
    }
}