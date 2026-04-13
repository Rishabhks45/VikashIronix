using VikashIronix_WebUI.AuthServices;
using VikashIronix_WebUI.Services.AuthServices.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using SendGrid.Helpers.Mail;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace VikashIronix_WebUI.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly NavigationManager _navigationManager;
        private readonly IHttpContextAccessor _context;

        public AuthService(
            IHttpClientFactory clientFactory,
            TokenValidator jwtValidator,
            NavigationManager navigationManager,
            IHttpContextAccessor context)
        {
            _clientFactory = clientFactory;
            _navigationManager = navigationManager;
            _context = context;
        }

        public async Task<UserLoginResponse?> UserLoginAsync(UserLoginRequest request)
        {
            var client = _clientFactory.CreateClient("VikashIronixApi");
            var result = await client.PostAsJsonAsync("api/auth/login", request);

            if (!result.IsSuccessStatusCode)
                return null;

            var response = await result.Content.ReadFromJsonAsync<UserLoginResponse>();
            return response;
        }
             
        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var client = _clientFactory.CreateClient("VikashIronixApi");
            var result = await client.PostAsJsonAsync("api/auth/forgot-password", new { Email = email });
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var client = _clientFactory.CreateClient("VikashIronixApi");
            var result = await client.PostAsJsonAsync("api/auth/reset-password", new { Token = token, NewPassword = newPassword });
            return result.IsSuccessStatusCode;
        }

        public async Task Logout()
        {
            await _context.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _navigationManager.NavigateTo("/");
        }
    }
}


