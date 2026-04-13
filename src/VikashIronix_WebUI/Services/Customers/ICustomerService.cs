using SharedKernel.DTOs.Customers;

namespace VikashIronix_WebUI.Services.Customers
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetCustomersAsync();
        Task CreateCustomerAsync(CustomerDto customer);
        Task UpdateCustomerAsync(CustomerDto customer);
        Task DeleteCustomerAsync(Guid id);
    }
}
