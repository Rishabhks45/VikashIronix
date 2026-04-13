using SharedKernel.DTOs.Users;
using WebApi.Features.Payroll.Infrastructure;

namespace WebApi.Features.Payroll;

public class PayrollService
{
    private readonly PayrollRepository _repository;

    public PayrollService(PayrollRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SalaryConfigurationDto>> GetSalaryConfigurationsAsync(CancellationToken token)
    {
        return await _repository.GetSalaryConfigurationsAsync(token);
    }

    public async Task UpsertSalaryConfigurationAsync(SalaryConfigurationDto config, CancellationToken token)
    {
        await _repository.UpsertSalaryConfigurationAsync(config, token);
    }

    public async Task DeleteSalaryConfigurationAsync(Guid id, CancellationToken token)
    {
        await _repository.DeleteSalaryConfigurationAsync(id, token);
    }
}
