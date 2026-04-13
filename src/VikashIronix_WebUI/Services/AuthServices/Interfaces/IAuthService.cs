using VikashIronix_WebUI.Services.AuthServices;

namespace VikashIronix_WebUI.Services.AuthServices.Interfaces
{
    public interface IAuthService
    {
        Task<UserLoginResponse?> UserLoginAsync(UserLoginRequest request);

        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        Task Logout();
    }
}
