using Microsoft.AspNetCore.Http;
using SharedKernel.DTOs;

namespace SharedKernel.Utilities.Middlewares;

public class UserContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, JwtHelper jwtHelper, ILogger<UserContextMiddleware> logger)
    {
        var authToken = ExtractToken(context);
        if (string.IsNullOrEmpty(authToken))
        {
            await next(context).ConfigureAwait(false);
            return;
        }

        try
        {
            var userContext = jwtHelper.ValidateAndCreateUserContext(authToken);
            if (!userContext.IsValid())
            {
                logger.LogWarning("Invalid user context: Missing user ID. Path: {Path}", context.Request.Path);
                throw new BadRequestException("Invalid user context: Missing user ID");
            }

            context.Items["UserContext"] = userContext;
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Unauthorized access while extracting user context. Path: {Path}", context.Request.Path);
            throw new UnauthorizedException(ex.Message, ex);
        }

        await next(context).ConfigureAwait(false);
    }

    private static string ExtractToken(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        return authHeader?
            .StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true
            ? authHeader.Substring("Bearer ".Length).Trim()
            : string.Empty;
    }
}

