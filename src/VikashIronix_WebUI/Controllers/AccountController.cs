using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VikashIronix_WebUI.Services.AuthServices.Interfaces;
using VikashIronix_WebUI.AuthServices;

namespace VikashIronix_WebUI.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly TokenValidator _tokenValidator;

    public AccountController(IAuthService authService, TokenValidator tokenValidator)
    {
        _authService = authService;
        _tokenValidator = tokenValidator;
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string token, [FromQuery] string? returnUrl, [FromQuery] bool isPersistent = false, [FromQuery] int expiresIn = 0)
    {
        if (string.IsNullOrEmpty(token))
        {
             return Redirect("/login?error=Invalid token");
        }

        // Validate Token to get Claims
        var principal = _tokenValidator.ValidateToken(token);
        if (principal != null)
        {
            var identity = (ClaimsIdentity)principal.Identity!;
            var claims = identity.Claims.ToList();

            // CLARIFICATION: The JWT from WebApi uses 'roles' (lowercase)
            // We need to ensure ASP.NET recognizes 'roles' for IsInRole()
            // and ALSO add standard ClaimTypes.Role for maximum compatibility with other components
            var roleClaims = claims.Where(c => c.Type == "roles" || c.Type == "role" || c.Type == ClaimTypes.Role);
            foreach (var roleClaim in roleClaims)
            {
                if (roleClaim.Type != ClaimTypes.Role)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
                }
            }

            // Create a new identity with the standard mapping
            var newIdentity = new ClaimsIdentity(
                claims, 
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role
            );

            // Add the name claim if it was using 'name' instead of standard URI
            if (!claims.Any(c => c.Type == ClaimTypes.Name))
            {
                var nameClaim = claims.FirstOrDefault(c => c.Type == "name" || c.Type == "unique_name");
                if (nameClaim != null)
                {
                    newIdentity.AddClaim(new Claim(ClaimTypes.Name, nameClaim.Value));
                }
            }

            var newPrincipal = new ClaimsPrincipal(newIdentity);
            newIdentity.AddClaim(new Claim("access_token", token));

             var expiryTime = expiresIn > 0 
                ? DateTime.UtcNow.AddSeconds(expiresIn) 
                : DateTime.UtcNow.AddHours(24);

             await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                newPrincipal, 
                new AuthenticationProperties 
                { 
                    IsPersistent = isPersistent,
                    ExpiresUtc = expiryTime
                });

             return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
        }
        
        return Redirect("/login?error=Invalid token or validation failed");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/login");
    }
}
