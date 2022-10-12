using Api.Models.Request;
using Api.Models.Responses;
using AutoMapper;
using Core.Abstractions;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IMapper _mapper;

    public CustomersController(ICustomerService customerService, IMapper mapper)
    {
        _customerService = customerService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("{customerId:int}")]
    public async Task<ActionResult<GetCustomerResponse>> GetCustomerAsync(int customerId)
    {
        var customerDto = await _customerService.GetCustomerById(customerId);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (customerDto is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
        }
        
        return _mapper.Map<CustomerDto, GetCustomerResponse>(customerDto!);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<CreateCustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request)
    {
        var customerDto = _mapper.Map<CreateCustomerRequest, CustomerDto>(request);
        customerDto = await _customerService.CreateCustomerAsync(customerDto);
        return _mapper.Map<CustomerDto, CreateCustomerResponse>(customerDto);
    }
}
