using Api.Models.Responses;
using AutoMapper;
using Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IMapper _mapper;

    public SubscriptionController(
        ISubscriptionService subscriptionService,
        IMapper mapper)
    {
        _subscriptionService = subscriptionService;
        _mapper = mapper;
    }

    /// <summary>
    /// Add a new email to subscription list
    /// </summary>
    /// <param name="email"></param>
    /// <returns> Email adding result </returns>
    /// <response code="200"> Email added successfully </response>
    /// <response code="400"> Email validation error </response>
    /// <response code="409"> Email is already exists </response>
    [HttpPost]
    [Route("/subscribe")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Subscribe([FromForm] string email)
    {
        var isEmailValid = EmailValidation.EmailValidator.Validate(email);
        if (!isEmailValid)
        {
            return BadRequest("Email is not valid!");
        }

        var isEmailAdded = await _subscriptionService.SubscribeAsync(email);
        return isEmailAdded
            ? Ok("Email added successfully!")
            : Conflict("This email is already exists!");
    }

    /// <summary>
    /// Send email with current course to all subscribers
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("/sendEmails")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Notify()
    {
        var result = await _subscriptionService.NotifyAsync();
        var response = _mapper.Map<SendEmailsResponse>(result);
        return Ok(response);
    }
}