using Customers.Queue.Processor.Models;
using Refit;

namespace Customers.Queue.Processor;

public interface ICustomersApi
{
    [Post("")]
    Task<CreateCustomerResponse> CreateCustomerAsync([Body] CreateCustomerRequest request);
}