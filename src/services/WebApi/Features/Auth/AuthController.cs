using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Features.Auth;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    AuthService service,
    ILogger<AuthController> logger
) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Login request received for email: {Email}", request.EmailId);
        var response = await service
            .AuthenticateUserAsync(request, cancellationToken)
            .ConfigureAwait(false);

        logger.LogInformation("Login successful. UserId: {UserId}", response.UserId);
        return Ok(response);
    }



    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Refresh token request received");
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("Refresh token request failed: Missing or invalid Authorization header");
            return Unauthorized("Authorization header with Bearer token is required");
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        var newToken = await service
            .GenerateRefreshTokenAsync(token, cancellationToken)
            .ConfigureAwait(false);

        logger.LogInformation("Refresh token successful. UserId: {UserId}",
            newToken.UserId);
        return Ok(newToken);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request, CancellationToken cancellationToken = default)
    {
        var result = await service.ForgotPasswordAsync(request, cancellationToken);
        if (result)
            return Ok(new { Message = "If the email exists, a reset link has been sent." });
        
        // Return OK even if failed to prevent enumeration, or BadRequest if you prefer.
        // For security, usually return OK. But logic returned 'false' for user not found.
        return Ok(new { Message = "If the email exists, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await service.ResetPasswordAsync(request, cancellationToken);
            if (result)
                return Ok(new { Message = "Password reset successfully." });
            
            return BadRequest(new { Message = "Failed to reset password." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}

