using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SharedKernel.Utilities.Helpers;

public class JwtHelper
{
    #region # Init

    public JwtHelper(IConfiguration configuration, ILogger<JwtHelper> logger)
    {
        _secretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        _issuer = configuration["Jwt:Issuer"] ?? "VikashIronix";
        _audience = configuration["Jwt:Audience"] ?? "VikashIronix";
        _expirationHours = configuration.GetValue("Jwt:ExpirationHours", 24);
        _refreshTokenExpirationDays = configuration.GetValue("Jwt:RefreshTokenExpirationDays", 7);

        // Pre-compute these for better performance 
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingKey,
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        _logger = logger;
        _logger.LogInformation("JwtHelper initialized. Issuer: {Issuer}, Audience: {Audience}, ExpirationHours: {ExpirationHours}",
            _issuer, _audience, _expirationHours);
    }

    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationHours;
    private readonly int _refreshTokenExpirationDays;

    private readonly SymmetricSecurityKey _signingKey;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly ILogger<JwtHelper> _logger;

    #endregion

    public void CreateToken(UserContext userContext)
    {
        if (userContext == null)
        {
            _logger.LogWarning("CreateToken failed: UserContext is null");
            throw new BadRequestException("UserContext cannot be null");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userContext.UserId.ToString()),
            new("name", userContext.UserName ?? string.Empty),
            new("email", userContext.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddHours(_expirationHours).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        };

        // Add Roles
        foreach (var role in userContext.Roles)
        {
            claims.Add(new Claim("roles", role));
        }

        // Add Scopes
        foreach (var scope in userContext.Scopes)
        {
            claims.Add(new Claim("scopes", scope));
        }

        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(_expirationHours),
            signingCredentials: credentials
        );

        userContext.Token = new JwtSecurityTokenHandler().WriteToken(token);
        userContext.TokenType = "Bearer";
        userContext.ExpiresIn = _expirationHours * 3600;
        userContext.RefreshToken = GenerateRefreshToken();
        userContext.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public UserContext ValidateAndCreateUserContext(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("ValidateAndCreateUserContext failed: Token is null or empty");
            throw new BadRequestException("Token cannot be null or empty");
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            if (jwtToken == null)
            {
                _logger.LogWarning("ValidateAndCreateUserContext failed: JWT token is null after validation");
                throw new UnauthorizedException("Invalid token format");
            }

            var claims = jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);
            if (claims == null || claims.Count == 0)
            {
                _logger.LogWarning("ValidateAndCreateUserContext failed: No claims found in token");
                throw new UnauthorizedException("Token contains no claims");
            }

            return UserContext.FromJwtClaims(claims);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "JWT token validation failed");
            throw new UnauthorizedException("Invalid or expired token", ex);
        }
    }

    public TokenValidationParameters GetTokenValidationParameters()
    {
        return _tokenValidationParameters;
    }
}

