using SharedKernel.DTOs.Holidays;

namespace VikashIronix_WebUI.Services.Holidays
{
    public interface IHolidayService
    {
        Task<List<HolidayDto>> GetHolidaysAsync();
        Task CreateHolidayAsync(HolidayDto holiday);
        Task UpdateHolidayAsync(HolidayDto holiday);
        Task DeleteHolidayAsync(Guid id);
    }
}
