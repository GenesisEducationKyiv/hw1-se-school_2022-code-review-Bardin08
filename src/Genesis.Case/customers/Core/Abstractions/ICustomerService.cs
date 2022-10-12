using Core.Dtos;

namespace Core.Abstractions;

public interface ICustomerService
{
    Task<CustomerDto> GetCustomerById(int customerId);
    Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto);
}