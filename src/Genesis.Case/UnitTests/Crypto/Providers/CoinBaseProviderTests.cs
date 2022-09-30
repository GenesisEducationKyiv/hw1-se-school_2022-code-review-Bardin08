using System.Threading.Tasks;
using Core.Contracts.Crypto.Abstractions;
using Core.Contracts.Crypto.Models;
using Integrations.Crypto.ExternalApis.CoinBase;
using Integrations.Crypto.Providers;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;
using GetExchangeRateResponse = Integrations.Crypto.Models.ExternalResponses.CoinBase.GetExchangeRateResponse;

namespace UnitTests.Crypto.Providers;

public class CoinBaseProviderTests
{
    private readonly ICryptoProvider _sut;
    private readonly Mock<ICoinBaseApiProxy> _coinBaseApiMock = new();

    public CoinBaseProviderTests()
    {
        _sut = new CoinBaseCryptoProvider(_coinBaseApiMock.Object);
    }
    
    [Fact]
    public async Task GetExchangeRateAsync_Binance_ReturnsExpectedValue()
    {
        // Arrange
        var apiResponse = new GetExchangeRateResponse
        {
            Data = new Integrations.Crypto.Models.ExternalResponses.CoinBase.Data {Rates = JToken.Parse("{\"UAH\":1.00}")}
        };
        _coinBaseApiMock.Setup(x => x.GetExchangeRateAsync(It.IsAny<Currency>()))
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
        _coinBaseApiMock.Setup(x => x.GetExchangeRateAsync(It.IsAny<Currency>()))
            .ReturnsAsync((GetExchangeRateResponse?)null);

        // Act
        var getExchangeRateResponse = await _sut.GetExchangeRateAsync(Currency.Btc, Currency.Uah);

        // Assert
        Assert.Equal(decimal.MinusOne, getExchangeRateResponse.ExchangeRate);
    }

}