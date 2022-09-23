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
    private readonly Mock<ICryptoProvider> _cryptoProviderMock = new();

    public ExchangeRateServiceTests()
    {
        _sut = new ExchangeRateService(_cryptoProviderMock.Object);
    }

    [Fact]
    public async Task GetExchangeRateAsync_ReturnsExpectedValue()
    {
        // Arrange
        _cryptoProviderMock.Setup(x => x.GetExchangeRateAsync(It.IsAny<Currency>(), It.IsAny<Currency>()))
            .ReturnsAsync(new Core.Crypto.Models.Responses.GetExchangeRateResponse {ExchangeRate = decimal.One});

        // Act
        var exchangeRate = await _sut.GetBtcToUahExchangeRateAsync();

        // Assert
        Assert.Equal(decimal.One, exchangeRate);
    }

    [Fact]
    public async Task GetExchangeRateAsync_ReturnsError()
    {
        // Arrange
        _cryptoProviderMock.Setup(x => x.GetExchangeRateAsync(It.IsAny<Currency>(), It.IsAny<Currency>()))
            .ReturnsAsync(new Core.Crypto.Models.Responses.GetExchangeRateResponse {ExchangeRate = decimal.MinusOne});

        // Act
        var response = await _sut.GetBtcToUahExchangeRateAsync();

        // Assert
        Assert.Equal(decimal.MinusOne, response);
    }
}