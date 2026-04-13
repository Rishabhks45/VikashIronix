using Microsoft.AspNetCore.Http;
using SharedKernel.DTOs;

namespace SharedKernel.Utilities.Extensions;

public static class UserContextExtensions
{
    public static UserContext GetUserContext(this IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor == null)
        {
            throw new InvalidOperationException("IHttpContextAccessor is not available");
        }

        var context = httpContextAccessor.HttpContext;
        if (context == null)
        {
            throw new InvalidOperationException("No HTTP context available - request context is not available");
        }

        return context.GetUserContext();
    }

    public static UserContext GetUserContext(this HttpContext context)
    {
        if (context == null)
        {
            throw new InvalidOperationException("No HTTP context available");
        }

        if (!context.Items.TryGetValue("UserContext", out var item) || item is not UserContext userContext)
        {
            throw new UnauthorizedException("No user context available - ensure user is authenticated and UserContextMiddleware is registered");
        }

        return userContext;
    }
}

