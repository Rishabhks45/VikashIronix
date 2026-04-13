using SharedKernel.DTOs.Holidays;
using WebApi.Features.Holidays.Infrastructure;

namespace WebApi.Features.Holidays;

public class HolidayService
{
    private readonly HolidayRepository _repository;

    public HolidayService(HolidayRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<HolidayDto>> GetHolidaysAsync(CancellationToken token)
    {
        return await _repository.GetHolidaysAsync(token);
    }

    public async Task CreateHolidayAsync(HolidayDto holiday, CancellationToken token)
    {
        await _repository.CreateHolidayAsync(holiday, token);
    }

    public async Task UpdateHolidayAsync(HolidayDto holiday, CancellationToken token)
    {
        await _repository.UpdateHolidayAsync(holiday, token);
    }

    public async Task DeleteHolidayAsync(Guid id, CancellationToken token)
    {
        await _repository.DeleteHolidayAsync(id, token);
    }
}
