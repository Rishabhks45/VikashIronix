using System;

namespace SharedKernel.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Name => $"{FirstName} {LastName}".Trim();
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string Role { get; set; } = string.Empty;
    public int? RoleId { get; set; }
    public string Password { get; set; } = string.Empty;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
}
