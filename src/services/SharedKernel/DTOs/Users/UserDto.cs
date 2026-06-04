using FluentValidation;
using System;

namespace SharedKernel.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Name => $"{FirstName} {LastName}".Trim();

    // Private backing field for storing email value
    private string _email = string.Empty;

    // Email property
    public string Email
    {
        // Return stored email value
        get => _email;

        // Convert email to lowercase before saving
        set => _email = value?.ToLower() ?? string.Empty;
    }

    public string? PhoneNumber { get; set; }

    public string? Bio { get; set; }

    public string? Location { get; set; }

    public string Role { get; set; } = string.Empty;

    public int? RoleId { get; set; }

    public string Password { get; set; } = string.Empty;

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; }
}

// FluentValidation Validator
public class UserDtoValidator : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(user => user.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(user => user.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(user => user.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(user => !string.IsNullOrWhiteSpace(user.PhoneNumber))
            .WithMessage("Invalid phone number format.");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .When(user => user.Id == Guid.Empty);

        RuleFor(user => user.RoleId)
            .NotEmpty().WithMessage("Role is required.");
    }
}