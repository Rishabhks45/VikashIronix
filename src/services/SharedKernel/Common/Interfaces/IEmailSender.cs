using Microsoft.AspNetCore.Http;

namespace SharedKernel.Common.Interfaces
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, string? cc = null, string? bcc = null, List<IFormFile>? attachments = null);
    }
}
