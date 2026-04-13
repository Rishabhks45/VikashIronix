using SharedKernel.DTOs.Holidays;
using System.Net.Http.Json;

namespace VikashIronix_WebUI.Services.Holidays
{
    public class HolidayService : IHolidayService
    {
        private readonly HttpClient _httpClient;

        public HolidayService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("VikashIronixApi");
        }

        public async Task<List<HolidayDto>> GetHolidaysAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<HolidayDto>>("api/Holiday");
                return response ?? new List<HolidayDto>();
            }
            catch (Exception)
            {
                return new List<HolidayDto>();
            }
        }

        public async Task CreateHolidayAsync(HolidayDto holiday)
        {
            await _httpClient.PostAsJsonAsync("api/Holiday", holiday);
        }

        public async Task UpdateHolidayAsync(HolidayDto holiday)
        {
            await _httpClient.PutAsJsonAsync("api/Holiday", holiday);
        }

        public async Task DeleteHolidayAsync(Guid id)
        {
            await _httpClient.DeleteAsync($"api/Holiday/{id}");
        }
    }
}
