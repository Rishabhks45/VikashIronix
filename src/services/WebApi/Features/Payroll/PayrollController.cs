using Microsoft.AspNetCore.Mvc;
using SharedKernel.DTOs.Users;

namespace WebApi.Features.Payroll;

[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly PayrollService _service;

    public PayrollController(PayrollService service)
    {
        _service = service;
    }

    [HttpGet("salary-configurations")]
    public async Task<ActionResult<List<SalaryConfigurationDto>>> GetSalaryConfigurations(CancellationToken token)
    {
        var result = await _service.GetSalaryConfigurationsAsync(token);
        return Ok(result);
    }

    [HttpPost("salary-configurations")]
    public async Task<ActionResult> UpsertSalaryConfiguration(SalaryConfigurationDto config, CancellationToken token)
    {
        await _service.UpsertSalaryConfigurationAsync(config, token);
        return Ok();
    }

    [HttpDelete("salary-configurations/{id}")]
    public async Task<ActionResult> DeleteSalaryConfiguration(Guid id, CancellationToken token)
    {
        await _service.DeleteSalaryConfigurationAsync(id, token);
        return Ok();
    }
}
