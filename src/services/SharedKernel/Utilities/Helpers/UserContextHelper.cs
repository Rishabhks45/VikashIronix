using Microsoft.AspNetCore.Http;
using SharedKernel.DTOs;

namespace SharedKernel.Utilities.Helpers;

/// <summary>
/// Helper class for safely accessing user context information
/// </summary>
public class UserContextHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Safely gets the current user ID from UserContext without throwing exceptions.
    /// Returns null if user is not authenticated or context is not available.
    /// </summary>
    public Guid? GetCurrentUserId()
    {
        var context = _httpContextAccessor?.HttpContext;
        if (context == null)
            return null;

        if (context.Items.TryGetValue("UserContext", out var item) && item is UserContext userContext && userContext.IsValid())
        {
            return userContext.UserId;
        }

        return null;
    }

    /// <summary>
    /// Safely gets the current UserContext without throwing exceptions.
    /// Returns null if user is not authenticated or context is not available.
    /// </summary>
    public UserContext? GetCurrentUserContext()
    {
        var context = _httpContextAccessor?.HttpContext;
        if (context == null)
            return null;

        if (context.Items.TryGetValue("UserContext", out var item) && item is UserContext userContext && userContext.IsValid())
        {
            return userContext;
        }

        return null;
    }

    /// <summary>
    /// Safely gets the current organization ID from UserContext without throwing exceptions.
    /// Returns null if user is not authenticated or context is not available.
    /// </summary>
    public Guid? GetCurrentOrganizationId()
    {
        var context = _httpContextAccessor?.HttpContext;
        if (context == null)
            return null;

     
        return null;
    }

    /// <summary>
    /// Checks if a user is currently authenticated
    /// </summary>
    public bool IsAuthenticated()
    {
        return GetCurrentUserId() != null;
    }
}

