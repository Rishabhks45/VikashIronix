namespace SharedKernel.Common.Interfaces;

public interface IEncryptionService
{
    string Encrypt(string plainText, string masterKey);
    Task<string> EncryptAsync(string plainText, string masterKey);
    string Decrypt(string encryptedText, string masterKey);
    Task<string> DecryptAsync(string encryptedText, string masterKey);
    string GenerateTemporaryPassword();
}
