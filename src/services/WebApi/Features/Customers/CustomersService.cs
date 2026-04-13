using SharedKernel.DTOs.Customers;
using WebApi.Features.Customers.Infrastructure;

namespace WebApi.Features.Customers;

public class CustomersService
{
    private readonly CustomersRepository _repository;

    public CustomersService(CustomersRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CustomerDto>> GetCustomersAsync(CancellationToken token)
    {
        return await _repository.GetCustomersAsync(token);
    }

    public async Task CreateCustomerAsync(CustomerDto customer, CancellationToken token)
    {
        await _repository.CreateCustomerAsync(customer, token);
    }

    public async Task UpdateCustomerAsync(CustomerDto customer, CancellationToken token)
    {
        await _repository.UpdateCustomerAsync(customer, token);
    }

    public async Task DeleteCustomerAsync(Guid id, CancellationToken token)
    {
        await _repository.DeleteCustomerAsync(id, token);
    }
}
