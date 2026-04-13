using SharedKernel.DTOs.Inventory;
using WebApi.Features.Inventory.Infrastructure;

namespace WebApi.Features.Inventory;

public class InventoryService
{
    private readonly InventoryRepository _repository;

    public InventoryService(InventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CreateCategoryAsync(CreateCategoryRequest request)
    {
        return await _repository.CreateCategoryAsync(request);
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
    {
        return await _repository.GetCategoriesAsync();
    }

    public async Task<bool> UpdateCategoryAsync(UpdateCategoryRequest request)
    {
        return await _repository.UpdateCategoryAsync(request);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        return await _repository.DeleteCategoryAsync(id);
    }

    public async Task<Guid> CreateMaterialAsync(CreateMaterialRequest request)
    {
        return await _repository.CreateMaterialAsync(request);
    }

    public async Task<IEnumerable<MaterialDto>> GetMaterialsAsync()
    {
        return await _repository.GetMaterialsAsync();
    }

    public async Task<bool> UpdateMaterialAsync(UpdateMaterialRequest request)
    {
        return await _repository.UpdateMaterialAsync(request);
    }

    public async Task<bool> DeleteMaterialAsync(Guid id)
    {
        return await _repository.DeleteMaterialAsync(id);
    }

    public async Task<bool> AddStockAsync(AddStockRequest request)
    {
        return await _repository.AddStockAsync(request);
    }
}
