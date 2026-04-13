using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace VikashIronix_WebUI.AuthServices
{
    public class JwtTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly NavigationManager _navigationManager;

        public JwtTokenHandler(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory clientFactory,
            NavigationManager navigationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
        {
            if (request.RequestUri!.AbsolutePath.Contains("refresh-token"))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var token = httpContext.User.FindFirst("access_token")?.Value;

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(content) && IsUnauthorizedResult(content))
                {
                    _navigationManager.NavigateTo("/perform-logout");
                   
                }
            }

            return response;
        }

        private bool IsUnauthorizedResult(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Check for "ErrorCode" (int)
                if (root.TryGetProperty("ErrorCode", out var errorCode))
                {
                    return errorCode.GetInt32() == 401;
                }
            }
            catch
            {
                // ignored
            }

            return false;
        }



        private async Task<string?> RefreshToken()
        {

            var client = _clientFactory.CreateClient("NexSchedApi");

            var result = await client.PostAsync("api/auth/refresh-token", null);

            if (!result.IsSuccessStatusCode)
                return null;

            var response = await result.Content.ReadFromJsonAsync<TokenResponse>();
            return response?.AccessToken;
        }
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}

