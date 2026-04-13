using SharedKernel.DTOs.Customers;
using System.Net.Http.Json;

namespace VikashIronix_WebUI.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _httpClient;

        public CustomerService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("VikashIronixApi");
        }

        public async Task<List<CustomerDto>> GetCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Customers");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<CustomerDto>>() ?? new List<CustomerDto>();
                }
            }
            catch { }
            return new List<CustomerDto>();
        }

        public async Task CreateCustomerAsync(CustomerDto customer)
        {
            await _httpClient.PostAsJsonAsync("api/Customers", customer);
        }

        public async Task UpdateCustomerAsync(CustomerDto customer)
        {
            await _httpClient.PutAsJsonAsync("api/Customers", customer);
        }

        public async Task DeleteCustomerAsync(Guid id)
        {
            await _httpClient.DeleteAsync($"api/Customers/{id}");
        }
    }
}
