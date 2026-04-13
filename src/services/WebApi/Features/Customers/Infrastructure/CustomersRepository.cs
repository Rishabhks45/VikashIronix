using Dapper;
using Microsoft.Data.SqlClient;
using SharedKernel.DTOs.Customers;
using SharedKernel.Services;
using SharedKernel.Settings;
using System.Data;

namespace WebApi.Features.Customers.Infrastructure;

public class CustomersRepository
{
    private readonly DbHelper _dbHelper;

    public EncryptionService EncryptionService { get; }
    public EncryptionSettings EncryptionSettings { get; }

    public CustomersRepository(DbHelper dbHelper, EncryptionService encryptionService, EncryptionSettings encryptionSettings)
    {
        _dbHelper = dbHelper;
        EncryptionService = encryptionService;
        EncryptionSettings = encryptionSettings;
    }

    public async Task<List<CustomerDto>> GetCustomersAsync(CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 1); // Get All

        var result = await connection.QueryAsync<CustomerDto>(
            "usp_Customer_Management",
            parameters,
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    public async Task CreateCustomerAsync(CustomerDto customer, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 2); // Insert
        parameters.Add("@FirstName", customer.FirstName);
        parameters.Add("@LastName", customer.LastName);
        parameters.Add("@PhoneNumber", customer.PhoneNumber);
        parameters.Add("@Email", customer.Email);
        parameters.Add("@RoleId", customer.RoleId);
        parameters.Add("@ShopName", customer.ShopName);
        parameters.Add("@ShopAddress", customer.ShopAddress);
        parameters.Add("@Bhada", customer.Bhada);
        parameters.Add("@DisplayIndex", customer.DisplayIndex);
        parameters.Add("@IsActive", customer.IsActive);


        //generate pass
        var ecryptedStoredPassword = await EncryptionService.EncryptAsync("Admin@123", EncryptionSettings.MasterKey);
        parameters.Add("@Password", ecryptedStoredPassword);

        await connection.ExecuteAsync(
            "usp_Customer_Management",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateCustomerAsync(CustomerDto customer, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 3); // Update
        parameters.Add("@Id", customer.Id);
        parameters.Add("@UserId", customer.UserId);
        parameters.Add("@FirstName", customer.FirstName);
        parameters.Add("@LastName", customer.LastName);
        parameters.Add("@PhoneNumber", customer.PhoneNumber);
        parameters.Add("@Email", customer.Email);
        parameters.Add("@RoleId", customer.RoleId);
        parameters.Add("@ShopName", customer.ShopName);
        parameters.Add("@ShopAddress", customer.ShopAddress);
        parameters.Add("@Bhada", customer.Bhada);
        parameters.Add("@DisplayIndex", customer.DisplayIndex);
        parameters.Add("@IsActive", customer.IsActive);

        await connection.ExecuteAsync(
            "usp_Customer_Management",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteCustomerAsync(Guid id, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 4); // Delete
        parameters.Add("@Id", id);

        await connection.ExecuteAsync(
            "usp_Customer_Management",
            parameters,
            commandType: CommandType.StoredProcedure);
    }
}
