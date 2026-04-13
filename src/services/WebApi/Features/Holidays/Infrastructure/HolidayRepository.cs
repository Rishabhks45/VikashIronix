using Dapper;
using SharedKernel.DTOs.Holidays;
using System.Data;

namespace WebApi.Features.Holidays.Infrastructure;

public class HolidayRepository
{
    private readonly DbHelper _dbHelper;

    public HolidayRepository(DbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<List<HolidayDto>> GetHolidaysAsync(CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 2); // 2 = Get All Active Holidays

        var result = await connection.QueryAsync<HolidayDto>("usp_Holidays", parameters, commandType: CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task CreateHolidayAsync(HolidayDto holiday, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 1); // 1 = Insert
        parameters.Add("@Name", holiday.Name);
        parameters.Add("@Date", holiday.Date);
        parameters.Add("@Type", (int)holiday.Type);
        parameters.Add("@Description", holiday.Description);
        parameters.Add("@IsActive", holiday.IsActive);

        await connection.ExecuteAsync(
            "usp_Holidays", 
            parameters, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateHolidayAsync(HolidayDto holiday, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 3); // 3 = Update
        parameters.Add("@Id", holiday.Id);
        parameters.Add("@Name", holiday.Name);
        parameters.Add("@Date", holiday.Date);
        parameters.Add("@Type", (int)holiday.Type);
        parameters.Add("@Description", holiday.Description);
        parameters.Add("@IsActive", holiday.IsActive);

        await connection.ExecuteAsync(
            "usp_Holidays", 
            parameters, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteHolidayAsync(Guid id, CancellationToken token)
    {
        using var connection = _dbHelper.GetSaasDB();
        
        var parameters = new DynamicParameters();
        parameters.Add("@QueryType", 4); // 4 = Soft Delete
        parameters.Add("@Id", id);

        await connection.ExecuteAsync(
            "usp_Holidays", 
            parameters, 
            commandType: CommandType.StoredProcedure);
    }
}
