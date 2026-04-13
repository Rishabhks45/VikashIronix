using SharedKernel.DTOs.Users;
using System.Net.Http.Json;

namespace VikashIronix_WebUI.Services.Users
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("VikashIronixApi");
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/Users");
                return response ?? new List<UserDto>();
            }
            catch (Exception ex)
            {
                return new List<UserDto>();
            }
        }

        public async Task CreateUserAsync(UserDto user)
        {
            await _httpClient.PostAsJsonAsync("api/Users", user);
        }

        public async Task UpdateUserAsync(UserDto user)
        {
            await _httpClient.PutAsJsonAsync("api/Users", user);
        }

        public async Task<List<RoleDto>> GetRolesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/Users/roles");
                return response ?? new List<RoleDto>();
            }
            catch
            {
                return new List<RoleDto>();
            }
        }

        public async Task CreateRoleAsync(RoleDto role)
        {
            await _httpClient.PostAsJsonAsync("api/Users/roles", role);
        }

        public async Task UpdateRoleAsync(RoleDto role)
        {
            await _httpClient.PutAsJsonAsync("api/Users/roles", role);
        }

        public async Task DeleteRoleAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/Users/roles/{id}");
        }

        public async Task<UserDto> GetProfileAsync(Guid userId)
        {
            var response = await _httpClient.GetFromJsonAsync<UserDto>($"api/Users/profile/{userId}");
            return response ?? new UserDto();
        }

        public async Task UpdateProfileAsync(UserDto profile)
        {
            await _httpClient.PutAsJsonAsync("api/Users/profile", profile);
        }

        public async Task ChangePasswordAsync(Guid userId, string newPassword)
        {
            await _httpClient.PostAsync($"api/Users/profile/change-password?userId={userId}&newPassword={newPassword}", null);
        }
    }
}
