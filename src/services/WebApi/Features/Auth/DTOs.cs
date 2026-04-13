using System.ComponentModel.DataAnnotations;

namespace WebApi.Features.Auth;

#region # User Login

public class UserLoginRequest
{
    [Required(ErrorMessage = "Email ID is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string EmailId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

public class UserLoginResponse
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int? UserType { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
}

public class ResponseUserDTO
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password{ get; set; } = string.Empty;
    public string PhoneNumber { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public int? UserType { get; set; }

    public Guid? MapId { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}


#endregion

