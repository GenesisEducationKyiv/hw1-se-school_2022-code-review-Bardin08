using System;
using System.Threading.Tasks;
using Api.Controllers;
using Core.Abstractions;
using HttpContextMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTests.Controllers;

public class RateControllerTests
{
    private readonly RateController _sut;
    private readonly Mock<IExchangeRateService> _exchangeRateServiceMock = new();

    public RateControllerTests()
    {
        _sut = new RateController(_exchangeRateServiceMock.Object)
        {
            ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
        };
    }

    [Fact]
    public async Task Get_WhenSuccess_ReturnsExchangeRate()
    {
        // Arrange
        _exchangeRateServiceMock.Setup(x => x.GetCurrentBtcToUahExchangeRateAsync())
            .ReturnsAsync(decimal.Parse("1.0"));

        // Act
        var result = await _sut.GetBtcExchangeRate();

        // Assert
        Assert.Equal(1.0m, result);
    }

    [Fact]
    public async Task Get_WhenException_ReturnsMinusOne()
    {
        // Arrange
        _exchangeRateServiceMock.Setup(x => x.GetCurrentBtcToUahExchangeRateAsync())
            .Throws<NullReferenceException>();

        // Act
        var result = await _sut.GetBtcExchangeRate();

        // Assert
        Assert.Equal(decimal.MinusOne, result);
    }
}