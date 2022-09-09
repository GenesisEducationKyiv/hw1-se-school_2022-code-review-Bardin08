using Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// This is a controller that is used to manually receive an exchange rate for BTC to UAH currencies.
/// </summary>
[ApiController]
[Route("rate")]
public class RateController : ControllerBase
{
    private readonly IExchangeRateService _exchangeRateService;

    public RateController(IExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }

    /// <summary>
    /// Get current BTC to UAH exchange rate
    /// </summary>
    /// <returns> Current BTC to UAH exchange rate </returns>
    /// <response code="200"> Returns current BTC to UAH exchange rate </response>
    /// <response code="400"> Any errors </response>
    [HttpGet]
    [Route("")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<decimal> GetBtcExchangeRate()
    {
        try
        {
            return await _exchangeRateService.GetBtcToUahExchangeRateAsync();
        }
        catch
        {
            HttpContext.Response.StatusCode = 400;
            return decimal.MinusOne;
        }
    }
}