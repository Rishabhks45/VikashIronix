using System.Security.Claims;

namespace SharedKernel.DTOs;

public class UserContext
{
    // User Info
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int UserType { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Scopes { get; set; } = new();

    // Token Info
    public string Token { get; internal set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; set; }
    public string TokenType { get; internal set; } = "Bearer";
    public int ExpiresIn { get; internal set; } = 86400;

    public static UserContext FromJwtClaims(Dictionary<string, string> claims)
    {
        if (claims == null)
        {
            throw new BadRequestException("Claims dictionary cannot be null");
        }

        return new UserContext
        {
            UserId = Guid.Parse(claims.GetValueOrDefault("sub", claims.GetValueOrDefault("user_id", Guid.Empty.ToString()))),
            UserName = claims.GetValueOrDefault("name", claims.GetValueOrDefault("user_name", string.Empty)) ?? string.Empty,
            Email = claims.GetValueOrDefault("email", string.Empty) ?? string.Empty,
            Role = claims.GetValueOrDefault(ClaimTypes.Role, string.Empty) ?? string.Empty,
        };
    }

    public bool IsValid()
    {
        return UserId != Guid.Empty;
    }
}

