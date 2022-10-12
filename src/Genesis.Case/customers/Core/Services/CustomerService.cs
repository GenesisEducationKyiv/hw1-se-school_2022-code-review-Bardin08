using System.Text;
using AutoMapper;
using Core.Abstractions;
using Core.Dtos;
using Data.Entities;
using Data.Providers;
using Integrations.RabbitMq.Abstractions;
using Integrations.RabbitMq.Models;

namespace Core.Services;

public class CustomerService : ICustomerService
{
    private readonly IJsonCustomersStorage _customerRepository;
    private readonly IAmqpProducer _amqpProducer;
    private readonly IMapper _mapper;

    public CustomerService(
        IJsonCustomersStorage customerRepository,
        IAmqpProducer amqpProducer,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _amqpProducer = amqpProducer;
        _mapper = mapper;
    }

    public async Task<CustomerDto> GetCustomerById(int customerId)
    {
        var customer = (await _customerRepository.ReadAsync(customerId))!;
        return _mapper.Map<Customer, CustomerDto>(customer);
    }

    public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto)
    {
        var customer = _mapper.Map<CustomerDto, Customer>(customerDto);
        customer = await _customerRepository.CreateAsync(customer);
        PublishCustomerCreationResponse(customer);
        return _mapper.Map<Customer, CustomerDto>(customer);
    }

    private void PublishCustomerCreationResponse(Customer customer)
    {
        _amqpProducer.SendMessages(new MessageModel[]
        {
            new()
            {
                RoutingKey = customer.Id > 0 ? "created_customers" : "failed_customers",
                Data = Encoding.UTF8.GetBytes(customer.Email!)
            }
        });
    }
}
