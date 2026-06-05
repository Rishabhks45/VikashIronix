using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;

namespace VikashIronix_WebUI.Services.FileUpload
{
    public class FileValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string Base64Data { get; set; } = string.Empty;
    }

    public interface IFileUploadService
    {
        Task<string> HandleFileUploadAsync(IBrowserFile file);
        Task<string> HandleFileUploadInByteAsync(byte[] file);
        Task<FileValidationResult> ValidateFileAsync(IBrowserFile file);
        Task<bool> DeleteFileAsync(string relativePath);
        bool FileExists(string relativePath);
    }
}
