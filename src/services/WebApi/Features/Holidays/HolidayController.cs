using Microsoft.AspNetCore.Mvc;
using SharedKernel.DTOs.Holidays;

namespace WebApi.Features.Holidays;

[ApiController]
[Route("api/[controller]")]
public class HolidayController : ControllerBase
{
    private readonly HolidayService _service;

    public HolidayController(HolidayService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<HolidayDto>>> GetHolidays(CancellationToken token)
    {
        var result = await _service.GetHolidaysAsync(token);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateHoliday(HolidayDto holiday, CancellationToken token)
    {
        await _service.CreateHolidayAsync(holiday, token);
        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateHoliday(HolidayDto holiday, CancellationToken token)
    {
        await _service.UpdateHolidayAsync(holiday, token);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteHoliday(Guid id, CancellationToken token)
    {
        await _service.DeleteHolidayAsync(id, token);
        return Ok();
    }
}
