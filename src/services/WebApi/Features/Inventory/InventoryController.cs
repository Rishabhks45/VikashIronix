using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.DTOs.Inventory;

namespace WebApi.Features.Inventory;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly InventoryService _inventoryService;

    public InventoryController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var id = await _inventoryService.CreateCategoryAsync(request);
        return Ok(new { Id = id, Message = "Category created successfully." });
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _inventoryService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpPut("categories")]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryRequest request)
    {
        var validator = new UpdateCategoryRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var success = await _inventoryService.UpdateCategoryAsync(request);
        return success ? Ok() : NotFound();
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var success = await _inventoryService.DeleteCategoryAsync(id);
        return success ? Ok() : NotFound();
    }

    [HttpPost("materials")]
    public async Task<IActionResult> CreateMaterial([FromBody] CreateMaterialRequest request)
    {
        var id = await _inventoryService.CreateMaterialAsync(request);
        return Ok(new { Id = id, Message = "Material created successfully." });
    }

    [HttpGet("materials")]
    public async Task<IActionResult> GetMaterials()
    {
        var materials = await _inventoryService.GetMaterialsAsync();
        return Ok(materials);
    }

    [HttpPut("materials")]
    public async Task<IActionResult> UpdateMaterial([FromBody] UpdateMaterialRequest request)
    {
        var validator = new UpdateMaterialRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var success = await _inventoryService.UpdateMaterialAsync(request);
        if (!success)
            return NotFound(new { message = "Material not found or update failed." });
            
        return Ok(new { message = "Material updated successfully." });
    }

    [HttpDelete("materials/{id}")]
    public async Task<IActionResult> DeleteMaterial(Guid id)
    {
        var success = await _inventoryService.DeleteMaterialAsync(id);
        if (!success)
            return NotFound(new { message = "Material not found or already deleted." });

        return Ok(new { message = "Material deleted successfully." });
    }

    [HttpPost("materials/stock")]
    public async Task<IActionResult> AddStock([FromBody] AddStockRequest request)
    {
        var validator = new AddStockRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var success = await _inventoryService.AddStockAsync(request);
        if (!success)
            return NotFound(new { message = "Material not found or stock update failed." });

        return Ok(new { message = "Stock added successfully." });
    }
}
