using System;

namespace SharedKernel.DTOs.Users;

public class SalaryConfigurationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public decimal MonthlySalary { get; set; }
    public DateTime EffectiveDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
