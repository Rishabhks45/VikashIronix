using SharedKernel.DTOs.Users;

namespace VikashIronix_WebUI.Services.Payroll
{
    public interface IPayrollService
    {
        Task<List<SalaryConfigurationDto>> GetSalaryConfigurationsAsync();
        Task UpsertSalaryConfigurationAsync(SalaryConfigurationDto config);
        Task DeleteSalaryConfigurationAsync(Guid id);
    }
}
