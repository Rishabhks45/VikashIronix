using SharedKernel.DTOs.Users;
using System.Net.Http.Json;

namespace VikashIronix_WebUI.Services.Payroll
{
    public class PayrollService : IPayrollService
    {
        private readonly HttpClient _httpClient;

        public PayrollService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("VikashIronixApi");
        }

        public async Task<List<SalaryConfigurationDto>> GetSalaryConfigurationsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<SalaryConfigurationDto>>("api/Payroll/salary-configurations");
                return response ?? new List<SalaryConfigurationDto>();
            }
            catch (Exception)
            {
                return new List<SalaryConfigurationDto>();
            }
        }

        public async Task UpsertSalaryConfigurationAsync(SalaryConfigurationDto config)
        {
            await _httpClient.PostAsJsonAsync("api/Payroll/salary-configurations", config);
        }

        public async Task DeleteSalaryConfigurationAsync(Guid id)
        {
            await _httpClient.DeleteAsync($"api/Payroll/salary-configurations/{id}");
        }
    }
}
