using Dapper;
using SharedKernel.DTOs.Inventory;
using SharedKernel.Services;
using System.Data;

namespace WebApi.Features.Inventory.Infrastructure;

public class InventoryRepository
{
    private readonly DbHelper _dbHelper;

    public InventoryRepository(DbHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public async Task<Guid> CreateCategoryAsync(CreateCategoryRequest request)
    {
        using var connection = _dbHelper.GetSaasDB();
        await connection.OpenAsync();

        var p = new DynamicParameters();
        p.Add("QueryType", 1);
        p.Add("Name", request.Name);
        p.Add("ParentId", request.ParentId);
        p.Add("HsnCode", request.HsnCode);
        p.Add("IsRoot", request.IsRoot);
        p.Add("Id", dbType: DbType.Guid, direction: ParameterDirection.Output);

        await connection.ExecuteAsync("usp_Inventory_Category", p, commandType: CommandType.StoredProcedure);
        return p.Get<Guid>("Id");
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
    {
        using var connection = _dbHelper.GetSaasDB();
        await connection.OpenAsync();

        var p = new DynamicParameters();
        p.Add("QueryType", 2);

        return await connection.QueryAsync<CategoryDto>("usp_Inventory_Category", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> UpdateCategoryAsync(UpdateCategoryRequest request)
    {
        using var connection = _dbHelper.GetSaasDB();
        await connection.OpenAsync();
        var p = new DynamicParameters();
        p.Add("QueryType", 3);
        p.Add("Id", request.Id);
        p.Add("Name", request.Name);
        p.Add("ParentId", request.ParentId);
        p.Add("HsnCode", request.HsnCode);
        p.Add("IsRoot", request.IsRoot);
        await connection.ExecuteAsync("usp_Inventory_Category", p, commandType: CommandType.StoredProcedure);
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        using var connection = _dbHelper.GetSaasDB();
        await connection.OpenAsync();
        var p = new DynamicParameters();
        p.Add("QueryType", 4);
        p.Add("Id", id);
        await connection.ExecuteAsync("usp_Inventory_Category", p, commandType: CommandType.StoredProcedure);
        return true;
    }

    public async Task<Guid> CreateMaterialAsync(CreateMaterialRequest request)
    {
        using var connection = _dbHelper.GetSaasDB();
        await connection.OpenAsync();

        var p = new DynamicParameters();
        p.Add("QueryType", 1);
        p.Add("CategoryId", request.CategoryId);
        p.Add("Name", request.Name);
        p.Add("Shape", request.Shape);
        p.Add("Thickness", request.Thickness);
        p.Add("Width", request.Width);
        p.Add("StandardLength", request.StandardLength);
        p.Add("SoldBy", request.SoldBy);
        p.Add("GlobalRate", request.GlobalRate);
        p.Add("Id", dbType: DbType.Guid, direction: ParameterDirection.Output);

        await connection.ExecuteAsync("usp_Inventory_Material", p, commandType: CommandType.StoredProcedure);
        return p.Get<Guid>("Id");
    }

    public async Task<IEnumerable<MaterialDto>> GetMaterialsAsync()
    {
        using var connection = _dbHelper.GetSaasDB();
        await connection.OpenAsync();

        var p = new DynamicParameters();
        p.Add("QueryType", 2);

        return await connection.QueryAsync<MaterialDto>("usp_Inventory_Material", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> UpdateMaterialAsync(UpdateMaterialRequest request)
    {
        using var connection = _dbHelper.GetSaasDB();
        await connection.OpenAsync();
        var p = new DynamicParameters();
        p.Add("QueryType", 3);
        p.Add("Id", request.Id);
        p.Add("CategoryId", request.CategoryId);
        p.Add("Name", request.Name);
        p.Add("Shape", request.Shape);
        p.Add("Thickness", request.Thickness);
        p.Add("Width", request.Width);
        p.Add("StandardLength", request.StandardLength);
        p.Add("SoldBy", request.SoldBy);
        p.Add("GlobalRate", request.GlobalRate);

        await connection.ExecuteAsync("usp_Inventory_Material", p, commandType: CommandType.StoredProcedure);
        return true; 
    }

    public async Task<bool> DeleteMaterialAsync(Guid id)
    {
        using var connection = _dbHelper.GetSaasDB();
        await connection.OpenAsync();
        var p = new DynamicParameters();
        p.Add("QueryType", 4);
        p.Add("Id", id);

        await connection.ExecuteAsync("usp_Inventory_Material", p, commandType: CommandType.StoredProcedure);
        return true;
    }

    public async Task<bool> AddStockAsync(AddStockRequest request)
    {
        using var connection = _dbHelper.GetSaasDB();
        await connection.OpenAsync();
        var p = new DynamicParameters();
        p.Add("QueryType", 5);
        p.Add("Id", request.MaterialId);
        p.Add("Thickness", request.AddedQuantity); // Thickness mapped to AddedQty in SP
        p.Add("GlobalRate", request.PurchaseRate); // GlobalRate mapped to PurchaseRate in SP

        await connection.ExecuteAsync("usp_Inventory_Material", p, commandType: CommandType.StoredProcedure);
        return true;
    }
}
