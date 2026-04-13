using WebApi.Features.Auth.Infrastructure;
using SharedKernel.Common.Interfaces;

namespace WebApi.Features.Auth;

public class AuthService
{
    private readonly AuthRepository authRepository;
    private readonly JwtHelper jwtHelper;
    private readonly ILogger<AuthService> logger;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public AuthService(
        AuthRepository authRepository,
        JwtHelper jwtHelper,
        ILogger<AuthService> logger,
        IEmailSender emailSender,
        IConfiguration configuration)
    {
        this.authRepository = authRepository;
        this.jwtHelper = jwtHelper;
        this.logger = logger;
        _emailSender = emailSender;
        _configuration = configuration;
    }

    public async Task<UserLoginResponse> AuthenticateUserAsync(UserLoginRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting to authenticate user with email: {Email}", request.EmailId);
        ValidationHelper.ValidateModelOrThrow(request);

        var userLoginResponse = await authRepository
            .AuthenticateUserAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (userLoginResponse == null)
        {
            logger.LogWarning("Authentication failed for email: {Email}", request.EmailId);
            throw new UnauthorizedException("Invalid email or password");
        }

        // Create user context and generate JWT token
        var scopes = await authRepository.GetScopesByRoleIdAsync(userLoginResponse.RoleId, cancellationToken);
        
        var userContext = new UserContext
        {
            UserId = userLoginResponse.UserId,
            UserName = userLoginResponse.UserName,
            Email = userLoginResponse.Email,
            Role = userLoginResponse.RoleName ?? string.Empty,
            UserType = userLoginResponse.UserType ?? 3,  // Default to Employee if null
            Roles = new List<string> { userLoginResponse.RoleName ?? "User" },
            Scopes = scopes
        };

        jwtHelper.CreateToken(userContext);
        
        // Save refresh token
        await SaveRefreshToken(userContext);

        // Update response with token information
        userLoginResponse.Token = userContext.Token;
        userLoginResponse.RefreshToken = userContext.RefreshToken;
        userLoginResponse.RefreshTokenExpiry = userContext.RefreshTokenExpiry;
        userLoginResponse.TokenType = userContext.TokenType;
        userLoginResponse.ExpiresIn = userContext.ExpiresIn;

        return userLoginResponse;
    }



    public async Task<bool> SaveRefreshToken(UserContext context)
    {
        return await authRepository.SaveRefreshToken(context.UserId, context.RefreshToken, context.RefreshTokenExpiry).ConfigureAwait(false);
    }
        
    public async Task<UserContext> GenerateRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting to refresh token");
        if (string.IsNullOrWhiteSpace(token))
        {
            logger.LogWarning("Refresh token attempt failed: Token is empty");
            throw new UnauthorizedException("Token is required");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var userContext = jwtHelper.ValidateAndCreateUserContext(token);
        if (userContext == null)
        {
            logger.LogWarning("Refresh token failed: UserContext is null after validation");
            throw new UnauthorizedException("Invalid token - unable to create user context");
        }

        logger.LogInformation("Token validated successfully. Generating new token for UserId: {UserId}",
            userContext.UserId);

        jwtHelper.CreateToken(userContext);
        
        // Save refresh token
        await SaveRefreshToken(userContext);

        return await Task.FromResult(userContext).ConfigureAwait(false);
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto request, CancellationToken cancellationToken)
    {
        var user = await authRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user == null) return false; // Don't reveal user existence

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("/", "_").Replace("+", "-").Replace("=", "");
        var expiry = DateTime.UtcNow.AddMinutes(15);

        await authRepository.SavePasswordResetTokenAsync(user.UserId, token, expiry, cancellationToken);

        // Link pointing to the Frontend Reset Page
        var frontendUrl = _configuration["ClientApp:BaseUrl"] ?? "https://localhost:7000"; 
        var resetLink = $"{frontendUrl}/reset-password?token={token}";
        
        var body = $"<h3>Password Reset Request</h3><p>Click the link below to reset your password:</p><a href='{resetLink}'>{resetLink}</a><p>This link expires in 15 minutes.</p>";

        return await _emailSender.SendEmailAsync(user.Email, "Reset Your Password", body);
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto request, CancellationToken cancellationToken)
    {
        var user = await authRepository.GetUserByResetTokenAsync(request.Token, cancellationToken);
        if (user == null) throw new Exception("Invalid or expired token.");

        var encryptedPassword = await authRepository.EncryptionService.EncryptAsync(request.NewPassword, authRepository.EncryptionSettings.MasterKey);

        var success = await authRepository.UpdateUserPasswordAsync(user.UserId, encryptedPassword, cancellationToken);
        if (success)
        {
            await authRepository.MarkTokenAsUsedAsync(request.Token, cancellationToken);
        }
        return success;
    }
}
