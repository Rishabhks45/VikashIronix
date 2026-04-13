using Dapper;
using Microsoft.Data.SqlClient;
using SharedKernel.Services;
using SharedKernel.Settings;
using System.Data;

namespace WebApi.Features.Auth.Infrastructure;

public class AuthRepository
{
    public DbHelper DbHelper { get; }
    public EncryptionService EncryptionService { get; }
    public EncryptionSettings EncryptionSettings { get; }
    public ILogger<AuthRepository> Logger { get; }

    public AuthRepository(DbHelper dbHelper, EncryptionService encryptionService, EncryptionSettings encryptionSettings, ILogger<AuthRepository> logger)
    {
        DbHelper = dbHelper;
        EncryptionService = encryptionService;
        EncryptionSettings = encryptionSettings;
        Logger = logger;
    }
    public async Task<UserLoginResponse?> AuthenticateUserAsync(UserLoginRequest request, CancellationToken cancellationToken = default)
    {
        using var connection = DbHelper.GetSaasDB();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        
        var p = new DynamicParameters();
        p.Add("QueryType", 2); // 2=GET_BY_EMAIL for authentication
        p.Add("Email", request.EmailId);

        var user = await connection.QueryFirstOrDefaultAsync<ResponseUserDTO>(
            "usp_Login_Users",
            p,
            commandType: CommandType.StoredProcedure
        ).ConfigureAwait(false);

        if (user == null)
            return null;

        // Decrypt the stored password and compare
        var decryptedStoredPassword = await EncryptionService.DecryptAsync(user.Password, EncryptionSettings.MasterKey);

        if (request.Password != decryptedStoredPassword)
            return null;

        return new UserLoginResponse
        {
            UserId = user.UserId,
            UserName = $"{user.FirstName} {user.LastName}".Trim(),
            Email = user.Email,
            RoleId = user.RoleId,
            RoleName = user.RoleName,
            UserType = user.UserType
        };
    }



 
    public async Task<bool> SaveRefreshToken(Guid userId, string refreshToken, DateTime expiry, CancellationToken cancellationToken = default)
    {
        using var connection = DbHelper.GetSaasDB();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var sql = @"
            UPDATE users 
            SET RefreshToken = @RefreshToken, 
                RefreshTokenExpiry = @RefreshTokenExpiry,
                LastLoginAt = SYSDATETIME(),
                updated_at = SYSDATETIME()
            WHERE id = @UserId";

        var p = new DynamicParameters();
        p.Add("UserId", userId);
        p.Add("RefreshToken", refreshToken);
        p.Add("RefreshTokenExpiry", expiry);

        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, p, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        return result > 0;
    }

    public async Task<ResponseUserDTO?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = DbHelper.GetSaasDB();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var p = new DynamicParameters();
        p.Add("QueryType", 1); // 6=CHECK_DUPLICATE_EMAIL
        p.Add("Id", userId);

        var result = await connection.QueryFirstOrDefaultAsync<ResponseUserDTO>(
            "usp_Login_Users",
            p,
            commandType: CommandType.StoredProcedure
        ).ConfigureAwait(false);


        return result;
    }
    public async Task<ResponseUserDTO?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        using var connection = DbHelper.GetSaasDB();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var p = new DynamicParameters();
        p.Add("QueryType", 2); 
        p.Add("Email", email);

        return await connection.QueryFirstOrDefaultAsync<ResponseUserDTO>(
            "usp_Login_Users",
            p,
            commandType: CommandType.StoredProcedure
        ).ConfigureAwait(false);
    }

    public async Task<bool> SavePasswordResetTokenAsync(Guid userId, string token, DateTime expiry, CancellationToken cancellationToken)
    {
        using var connection = DbHelper.GetSaasDB();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var p = new DynamicParameters();
        p.Add("QueryType", 1); // 1 = Save Token
        p.Add("UserId", userId);
        p.Add("ResetToken", token);
        p.Add("ExpiresAtUtc", expiry);

        var result = await connection.ExecuteAsync(
            "usp_Auth_PasswordReset", 
            p, 
            commandType: CommandType.StoredProcedure
        ).ConfigureAwait(false);
        
        return result > 0;
    }

    public async Task<ResponseUserDTO?> GetUserByResetTokenAsync(string token, CancellationToken cancellationToken)
    {
        using var connection = DbHelper.GetSaasDB();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var p = new DynamicParameters();
        p.Add("QueryType", 2); // 2 = Get User by Token
        p.Add("ResetToken", token);

        return await connection.QueryFirstOrDefaultAsync<ResponseUserDTO>(
            "usp_Auth_PasswordReset",
            p,
            commandType: CommandType.StoredProcedure
        ).ConfigureAwait(false);
    }

    public async Task<bool> MarkTokenAsUsedAsync(string token, CancellationToken cancellationToken)
    {
        using var connection = DbHelper.GetSaasDB();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var p = new DynamicParameters();
        p.Add("QueryType", 3); // 3 = Mark Used
        p.Add("ResetToken", token);

        var result = await connection.ExecuteAsync(
            "usp_Auth_PasswordReset",
            p,
            commandType: CommandType.StoredProcedure
        ).ConfigureAwait(false);
        
        return result > 0;
    }

    public async Task<bool> UpdateUserPasswordAsync(Guid userId, string encryptedPassword, CancellationToken cancellationToken)
    {
        using var connection = DbHelper.GetSaasDB();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var p = new DynamicParameters();
        p.Add("QueryType", 4); // 4 = Update Password
        p.Add("UserId", userId);
        p.Add("Password", encryptedPassword);

        var result = await connection.ExecuteAsync(
            "usp_Auth_PasswordReset",
            p,
            commandType: CommandType.StoredProcedure
        ).ConfigureAwait(false);
        
        return result > 0;
    }

    public async Task<List<string>> GetScopesByRoleIdAsync(int roleId, CancellationToken cancellationToken)
    {
        using var connection = DbHelper.GetSaasDB();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var scopes = await connection.QueryAsync<string>(
            "usp_Auth_GetScopes",
            new { RoleId = roleId },
            commandType: CommandType.StoredProcedure
        ).ConfigureAwait(false);

        return scopes.ToList();
    }
}

