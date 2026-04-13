using System.ComponentModel.DataAnnotations;

namespace VikashIronix_WebUI.Services.AuthServices
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "Email ID is required")]
        public string EmailId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public class UserLoginResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiry { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; }
    }

    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiry { get; set; }
        public string TokenType { get; set; } = string.Empty; 
        public int ExpiresIn { get; set; }                    
        public string UserId { get; set; } = string.Empty;    
        public string UserName { get; set; } = string.Empty;  
    }

    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

