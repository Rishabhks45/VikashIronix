using Microsoft.AspNetCore.Mvc;
using SharedKernel.DTOs.Customers;

namespace WebApi.Features.Customers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomersService _service;

    public CustomersController(CustomersService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers(CancellationToken token)
    {
        var customers = await _service.GetCustomersAsync(token);
        return Ok(customers);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerDto customer, CancellationToken token)
    {
        await _service.CreateCustomerAsync(customer, token);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDto customer, CancellationToken token)
    {
        await _service.UpdateCustomerAsync(customer, token);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(Guid id, CancellationToken token)
    {
        await _service.DeleteCustomerAsync(id, token);
        return Ok();
    }
}
