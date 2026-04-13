using SharedKernel.Common.Interfaces;
using SharedKernel.Settings;

namespace SharedKernel.Services;

/// <summary>
/// Helper service to simplify encryption/decryption operations using the configured master key
/// </summary>
public class EncryptionHelper
{
    private readonly IEncryptionService _encryptionService;
    private readonly string _masterKey;

    public EncryptionHelper(IEncryptionService encryptionService, EncryptionSettings encryptionSettings)
    {
        _encryptionService = encryptionService;
        _masterKey = encryptionSettings.MasterKey;
    }

    /// <summary>
    /// Encrypts text using the configured master key
    /// </summary>
    public async Task<string> EncryptAsync(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            return string.Empty;

        return await _encryptionService.EncryptAsync(plainText, _masterKey);
    }

    /// <summary>
    /// Decrypts text using the configured master key
    /// </summary>
    public async Task<string> DecryptAsync(string encryptedText)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
            return string.Empty;

        return await _encryptionService.DecryptAsync(encryptedText, _masterKey);
    }

    /// <summary>
    /// Encrypts text synchronously using the configured master key
    /// </summary>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            return string.Empty;

        return _encryptionService.Encrypt(plainText, _masterKey);
    }

    /// <summary>
    /// Decrypts text synchronously using the configured master key
    /// </summary>
    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
            return string.Empty;

        return _encryptionService.Decrypt(encryptedText, _masterKey);
    }

    /// <summary>
    /// Generates a cryptographically secure temporary password
    /// </summary>
    public string GenerateTemporaryPassword()
    {
        return _encryptionService.GenerateTemporaryPassword();
    }

    /// <summary>
    /// Generates a temporary password and returns both plain and encrypted versions
    /// </summary>
    public async Task<(string PlainPassword, string EncryptedPassword)> GenerateAndEncryptPassword()
    {
        string tempPassword = _encryptionService.GenerateTemporaryPassword();
        string encrypted = await EncryptAsync(tempPassword);
        return (tempPassword, encrypted);
    }
}
