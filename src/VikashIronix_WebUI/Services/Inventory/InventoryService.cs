using SharedKernel.DTOs.Inventory;
using System.Net.Http.Json;

namespace VikashIronix_WebUI.Services.Inventory;

public interface IInventoryService
{
    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<Guid> CreateCategoryAsync(CreateCategoryRequest request);
    Task<bool> UpdateCategoryAsync(UpdateCategoryRequest request);
    Task<bool> DeleteCategoryAsync(Guid id);
    Task<List<MaterialDto>> GetMaterialsAsync();
    Task<Guid> CreateMaterialAsync(CreateMaterialRequest request);
    Task<bool> UpdateMaterialAsync(UpdateMaterialRequest request);
    Task<bool> DeleteMaterialAsync(Guid id);
    Task<bool> AddStockAsync(AddStockRequest request);
}

public class InventoryService : IInventoryService
{
    private readonly HttpClient _httpClient;

    public InventoryService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("VikashIronixApi");
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/v1/Inventory/categories");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new List<CategoryDto>();
            }
        }
        catch { }
        return new List<CategoryDto>();
    }

    public async Task<Guid> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/Inventory/categories", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task<bool> UpdateCategoryAsync(UpdateCategoryRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("api/v1/Inventory/categories", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/v1/Inventory/categories/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<MaterialDto>> GetMaterialsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/v1/Inventory/materials");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MaterialDto>>() ?? new List<MaterialDto>();
            }
        }
        catch { }
        return new List<MaterialDto>();
    }

    public async Task<Guid> CreateMaterialAsync(CreateMaterialRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/Inventory/materials", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateResponse>();
        return result?.Id ?? Guid.Empty;
    }

    public async Task<bool> UpdateMaterialAsync(UpdateMaterialRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("api/v1/Inventory/materials", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteMaterialAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/v1/Inventory/materials/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AddStockAsync(AddStockRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/Inventory/materials/stock", request);
        return response.IsSuccessStatusCode;
    }

    private class CreateResponse { public Guid Id { get; set; } }
}
