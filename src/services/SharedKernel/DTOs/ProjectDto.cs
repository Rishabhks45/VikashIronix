using System.ComponentModel.DataAnnotations;
using SharedKernel.Enums;

namespace SharedKernel.DTOs;

#region Project DTOs

public class RequestProjectDto
{
    public Guid ProjectId { get; set; }

    public Guid OrganizationId { get; set; }

    [Required(ErrorMessage = "Project name is required")]
    [MaxLength(200)]
    public string ProjectName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Client is required")]
    public Guid? ClientId { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;

    [MaxLength(200)]
    public string? HubStaffProjectName { get; set; }

    // Audit Fields
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ResponseProjectDto
{
    public Guid ProjectId { get; set; }
    public Guid OrganizationId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ProjectStatus Status { get; set; }
    public string? HubStaffProjectName { get; set; }

    // Computed properties
    public string Duration => GetDuration();
    public string StatusDisplayName => Status.GetDisplayName();

    // Audit Fields
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Pagination
    public int TotalCount { get; set; }

    private string GetDuration()
    {
        var start = StartDate;
        var end = EndDate ?? DateTime.UtcNow;
        var duration = end - start;
        
        if (duration.TotalDays < 30)
            return $"{(int)duration.TotalDays} days";
        
        var months = (int)(duration.TotalDays / 30);
        if (months < 12)
            return $"{months} month{(months > 1 ? "s" : "")}";
        
        var years = months / 12;
        var remainingMonths = months % 12;
        return remainingMonths > 0 
            ? $"{years} year{(years > 1 ? "s" : "")} {remainingMonths} month{(remainingMonths > 1 ? "s" : "")}"
            : $"{years} year{(years > 1 ? "s" : "")}";
    }
}

#endregion
