using System.Threading.Tasks;
using Core.Contracts.Crypto.Abstractions;
using Core.Contracts.Crypto.Models;
using Core.Services;
using Moq;
using Xunit;

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
            .ReturnsAsync(new GetExchangeRateResponse {ExchangeRate = decimal.One});

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
            .ReturnsAsync(new GetExchangeRateResponse {ExchangeRate = decimal.MinusOne});

        // Act
        var response = await _sut.GetBtcToUahExchangeRateAsync();

        // Assert
        Assert.Equal(decimal.MinusOne, response);
    }
}