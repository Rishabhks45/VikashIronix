using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace VikashIronix_WebUI.AuthServices
{
    public class CookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJSRuntime? _jsRuntime;

        public CookieService(IHttpContextAccessor httpContextAccessor, IJSRuntime? jsRuntime = null)
        {
            _httpContextAccessor = httpContextAccessor;
            _jsRuntime = jsRuntime;
        }

        public async Task Set(string key, string value, int days)
        {
            // For Blazor Server, try to set cookie via HttpContext (server-side)
            // But only if response hasn't started yet
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && !httpContext.Response.HasStarted)
            {
                try
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(days),
                        Path = "/"
                    };
                    httpContext.Response.Cookies.Append(key, value, cookieOptions);
                }
                catch (InvalidOperationException)
                {
                    // Response has started, fall through to JS interop
                }
            }
            
            // Use JS interop for client-side cookie setting (works even if response started)
            // Note: JS-set cookies won't have HttpOnly flag, but will work for client-side access
            if (_jsRuntime != null)
            {
                try
                {
                    await InvokeJsVoidAsync("setCookie", key, value, days);
                }
                catch
                {
                    // JS interop isn't available (e.g., prerender). Ignore.
                }
            }
        }

        public async Task<string?> Get(string key)
        {
            // For Blazor Server, read cookie from HttpContext (works during prerendering)
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue(key, out var cookieValue))
            {
                return cookieValue;
            }

            // Fallback to JS interop if cookie not found in HttpContext
            // This handles cases where cookie was set via JavaScript
            if (_jsRuntime != null)
            {
                try
                {
                    return await InvokeJsFuncAsync("getCookie", key);
                }
                catch
                {
                    // JS interop not available (e.g., prerender). Return null.
                }
            }

            return null;
        }

        public async Task Remove(string key)
        {
            // For Blazor Server, try to remove cookie via HttpContext (server-side)
            // But only if response hasn't started yet
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && !httpContext.Response.HasStarted)
            {
                try
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(1),
                        Path = "/"
                    };
                    httpContext.Response.Cookies.Delete(key, cookieOptions);
                }
                catch (InvalidOperationException)
                {
                    // Response has started, fall through to JS interop
                }
            }
            
            // Use JS interop for client-side cookie removal (works even if response started)
            if (_jsRuntime != null)
            {
                try
                {
                    await InvokeJsVoidAsync("removeCookie", key);
                }
                catch
                {
                    // JS interop isn't available (e.g., prerender). Ignore.
                }
            }
        }

        private async Task InvokeJsVoidAsync(string identifier, params object[] args)
        {
            if (_jsRuntime == null) return;
            
            try
            {
                await _jsRuntime.InvokeVoidAsync(identifier, args);
            }
            catch
            {
                // JS interop isn't available (e.g., prerender). Ignore.
            }
        }

        private async Task<string?> InvokeJsFuncAsync(string identifier, params object[] args)
        {
            if (_jsRuntime == null) return null;
            
            try
            {
                return await _jsRuntime.InvokeAsync<string>(identifier, args);
            }
            catch
            {
                return null;
            }
        }
    }
}

