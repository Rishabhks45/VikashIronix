using Dapper;
using SharedKernel.DTOs.Users;
using SharedKernel.Services;
using SharedKernel.Settings;
using System.Data;

namespace WebApi.Features.Users.Infrastructure;

public class UsersRepository
{
    private readonly DbHelper _dbHelper;
    public EncryptionService EncryptionService { get; }
    public EncryptionSettings EncryptionSettings { get; }
    public UsersRepository(DbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<List<UserDto>> GetUsersAsync(CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 1); // 1 = Get All Users

        var result = await connection.QueryAsync<UserDto>("usp_User_Management", parameters, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task CreateUserAsync(UserDto user, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 2); // 2 = Insert
        parameters.Add("@FirstName", user.FirstName);
        parameters.Add("@LastName", user.LastName);
        parameters.Add("@Email", user.Email);
        parameters.Add("@RoleId", user.RoleId);
        parameters.Add("@IsActive", user.IsActive);
        var encryptedPassword = await EncryptionService.EncryptAsync(user.Password, EncryptionSettings.MasterKey);
        
        parameters.Add("@PasswordHash", encryptedPassword);

        await connection.ExecuteAsync(
            "usp_User_Management", 
            parameters, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateUserAsync(UserDto user, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 3); // 3 = Update
        parameters.Add("@Id", user.Id);
        parameters.Add("@FirstName", user.FirstName);
        parameters.Add("@LastName", user.LastName);
        parameters.Add("@Email", user.Email);
        parameters.Add("@RoleId", user.RoleId);
        parameters.Add("@IsActive", user.IsActive);

        await connection.ExecuteAsync(
            "usp_User_Management", 
            parameters, 
            commandType: CommandType.StoredProcedure);
    }


    public async Task<List<RoleDto>> GetRolesAsync(CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 1); // 1 = Get All Roles

        var result = await connection.QueryAsync<RoleDto>(
            "usp_Role_Management", 
            parameters, 
            commandType: CommandType.StoredProcedure);
            
        return result.ToList();
    }

    public async Task CreateRoleAsync(RoleDto role, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 2); // 2 = Insert
        parameters.Add("@Name", role.Name);
        parameters.Add("@Description", role.Description);

        await connection.ExecuteAsync(
            "usp_Role_Management", 
            parameters, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateRoleAsync(RoleDto role, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 3); // 3 = Update
        parameters.Add("@Id", role.Id);
        parameters.Add("@Name", role.Name);
        parameters.Add("@Description", role.Description);

        await connection.ExecuteAsync(
            "usp_Role_Management", 
            parameters, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteRoleAsync(int id, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 4); // 4 = Delete
        parameters.Add("@Id", id);

        await connection.ExecuteAsync(
            "usp_Role_Management", 
            parameters, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<UserDto?> ManageProfileAsync(string action, Guid? userId = null, UserDto? profile = null, string? newPassword = null)
    {
        using var connection = _dbHelper.GetSaasDB();
        var parameters = new DynamicParameters();
        parameters.Add("@Action", action);
        parameters.Add("@UserId", userId);

        if (profile != null)
        {
            parameters.Add("@FirstName", profile.FirstName);
            parameters.Add("@LastName", profile.LastName);
            parameters.Add("@Email", profile.Email);
            parameters.Add("@PhoneNumber", profile.PhoneNumber);
            parameters.Add("@Bio", profile.Bio);
            parameters.Add("@Location", profile.Location);
        }

        if (!string.IsNullOrEmpty(newPassword))
        {
            // Simple Base64 mock as per existing pattern in this repo
            string encrypted = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(newPassword));
            parameters.Add("@PasswordHash", encrypted);
        }

        if (action == "GET")
        {
            return await connection.QueryFirstOrDefaultAsync<UserDto>(
                "usp_User_Profile_Management", 
                parameters, 
                commandType: CommandType.StoredProcedure);
        }
        else
        {
            await connection.ExecuteAsync(
                "usp_User_Profile_Management", 
                parameters, 
                commandType: CommandType.StoredProcedure);
            return null;
        }
    }
}
