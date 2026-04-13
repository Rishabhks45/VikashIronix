using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SharedKernel.DTOs.Billing;

namespace VikashIronix_WebUI.Services.Bills
{
    public class BillService : IBillService
    {
        private readonly HttpClient _httpClient;
        
        public BillService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("VikashIronixApi");
        }

        public async Task<List<BillDto>> GetBillsAsync()
        {
            try
            {
                var bills = await _httpClient.GetFromJsonAsync<List<BillDto>>("api/Bills");
                return bills ?? new List<BillDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving bills: {ex.Message}");
                return new List<BillDto>();
            }
        }

        public async Task<BillDto> GetBillByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<BillDto>($"api/Bills/{id}");
        }

        public async Task<BillSaveResult> CreateBillAsync(BillDto bill)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Bills", bill);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BillSaveResult>() 
                   ?? new BillSaveResult();
        }

        public async Task<Guid> UpdateBillAsync(BillDto bill)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Bills/{bill.Id}", bill);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Guid>();
        }

        public async Task<bool> DeleteBillAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/Bills/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
