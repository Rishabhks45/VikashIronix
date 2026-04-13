using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using SharedKernel.Settings;

namespace SharedKernel.Services;

public class EmailService : SharedKernel.Common.Interfaces.IEmailSender
{
    private readonly SendGridSettings _sendGridSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SendGridSettings> sendGridSettings, ILogger<EmailService> logger)
    {
        _sendGridSettings = sendGridSettings.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, string? cc = null, string? bcc = null, List<IFormFile>? attachments = null)
    {
        try
        {
            // 1. Create MimeMessage (Standard Format)
            var mimeMessage = CreateMimeMessage(toEmail, subject, body, attachments);

            // 2. Validate Configuration
            if (!ValidateConfiguration(out var apiKey, out var fromEmail, out var fromName))
            {
                // Fallback to disk if config is invalid
                return await SaveToDiskAsync(mimeMessage);
            }

            // 3. Send via SendGrid
            return await SendViaSendGridAsync(mimeMessage, apiKey, fromEmail, fromName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Email sending failed: {ex.Message}. Email to: {toEmail} | Subject: {subject}");
            // Fallback on unexpected errors
            await SaveToDiskAsync(toEmail, subject, body); 
            return false;
        }
    }

    private MimeMessage CreateMimeMessage(string toEmail, string subject, string body, List<IFormFile>? attachments)
    {
        var emailMessage = new MimeMessage();
        
        // We set 'To' here for the MimeMessage logging/structure
        // 'From' is set later based on config to ensure we always use the latest settings
        
        emailMessage.To.Add(MailboxAddress.Parse(toEmail));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body,
            TextBody = body // Fallback text
        };

        if (attachments != null)
        {
            foreach (var file in attachments)
            {
                using var ms = new MemoryStream();
                file.CopyTo(ms);
                bodyBuilder.Attachments.Add(file.FileName, ms.ToArray(), ContentType.Parse(file.ContentType));
            }
        }

        emailMessage.Body = bodyBuilder.ToMessageBody();
        return emailMessage;
    }

    private bool ValidateConfiguration(out string apiKey, out string fromEmail, out string fromName)
    {
        apiKey = string.Empty;
        fromEmail = string.Empty;
        fromName = string.Empty;

        if (_sendGridSettings == null)
        {
            _logger.LogError("SendGridSettings is null.");
            return false;
        }

        apiKey = _sendGridSettings.APIKey?.Trim() ?? "";
        fromEmail = _sendGridSettings.FromEmail;
        fromName = _sendGridSettings.FromName;

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogError("SendGrid API Key is missing.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(fromEmail))
        {
            _logger.LogError("FromEmail is missing in settings.");
            return false;
        }

        return true;
    }

    private async Task<bool> SendViaSendGridAsync(MimeMessage mimeMessage, string apiKey, string fromEmail, string fromName)
    {
        try
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            
            // Map MimeMessage recipients to SendGrid
            // Note: Currently handling single recipient per SendEmailAsync contract, but loop supports future expansion
            bool allSuccess = true;
            
            foreach (var recipient in mimeMessage.To.Mailboxes)
            {
                var sendGridMessage = MailHelper.CreateSingleEmail(
                    from,
                    new EmailAddress(recipient.Address, recipient.Name),
                    mimeMessage.Subject,
                    mimeMessage.TextBody,
                    mimeMessage.HtmlBody
                );

                // Add Attachments
                foreach (var attachment in mimeMessage.Attachments.OfType<MimePart>())
                {
                    using var ms = new MemoryStream();
                    attachment.Content.DecodeTo(ms);
                    sendGridMessage.AddAttachment(
                        attachment.FileName,
                        Convert.ToBase64String(ms.ToArray()),
                        attachment.ContentType.MimeType,
                        "attachment"
                    );
                }

                var response = await client.SendEmailAsync(sendGridMessage);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Email sent successfully to {recipient.Address} | Subject: {mimeMessage.Subject}");
                }
                else
                {
                    var body = await response.Body.ReadAsStringAsync();
                    _logger.LogError($"SendGrid Failed: {response.StatusCode}. Email to: {recipient.Address} | Subject: {mimeMessage.Subject} | Details: {body}");
                    
                    allSuccess = false;
                    await SaveToDiskAsync(mimeMessage);
                }
            }

            return allSuccess;
        }
        catch (Exception ex)
        {
            _logger.LogError($"SendGrid Exception: {ex.Message}");
            await SaveToDiskAsync(mimeMessage);
            return false;
        }
    }

    private async Task<bool> SaveToDiskAsync(MimeMessage mimeMessage)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"--- EMAIL SENT AT {DateTime.Now} ---");
        sb.AppendLine($"To: {string.Join(", ", mimeMessage.To)}");
        sb.AppendLine($"Subject: {mimeMessage.Subject}");
        sb.AppendLine("--- BODY ---");
        sb.AppendLine(mimeMessage.TextBody ?? mimeMessage.HtmlBody);
        
        return await WriteToFileAsync(sb.ToString());
    }

    // Overload for simple plain text fallback
    private async Task<bool> SaveToDiskAsync(string to, string subject, string body)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"--- EMAIL SENT AT {DateTime.Now} ---");
        sb.AppendLine($"To: {to}");
        sb.AppendLine($"Subject: {subject}");
        sb.AppendLine("--- BODY ---");
        sb.AppendLine(body);
        
        return await WriteToFileAsync(sb.ToString());
    }

    private async Task<bool> WriteToFileAsync(string content)
    {
        try
        {
            var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sent_emails");
            Directory.CreateDirectory(logDirectory);

            var filename = $"email_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid()}.txt";
            var filePath = Path.Combine(logDirectory, filename);

            await File.WriteAllTextAsync(filePath, content);
            _logger.LogWarning($"[DevFallback] Email saved to: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to save email to disk: {ex.Message}");
            return false;
        }
    }
}

