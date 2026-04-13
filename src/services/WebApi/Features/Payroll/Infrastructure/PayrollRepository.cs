using Dapper;
using SharedKernel.DTOs.Users;
using System.Data;

namespace WebApi.Features.Payroll.Infrastructure;

public class PayrollRepository
{
    private readonly DbHelper _dbHelper;

    public PayrollRepository(DbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<List<SalaryConfigurationDto>> GetSalaryConfigurationsAsync(CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 1);
        var result = await connection.QueryAsync<SalaryConfigurationDto>("usp_Salary_Management", parameters, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task UpsertSalaryConfigurationAsync(SalaryConfigurationDto config, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 2);
        parameters.Add("@Id", config.Id == Guid.Empty ? (Guid?)null : config.Id);
        parameters.Add("@UserId", config.UserId);
        parameters.Add("@MonthlySalary", config.MonthlySalary);
        parameters.Add("@EffectiveDate", config.EffectiveDate);
        parameters.Add("@IsActive", config.IsActive);

        await connection.ExecuteAsync("usp_Salary_Management", parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteSalaryConfigurationAsync(Guid id, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 4);
        parameters.Add("@Id", id);
        await connection.ExecuteAsync("usp_Salary_Management", parameters, commandType: CommandType.StoredProcedure);
    }
}
