using SharedKernel.Common.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SharedKernel.Services;

public class EncryptionService : IEncryptionService
{
    private const int KeySize = 32;
    private const int IvSize = 12;
    private const int SaltSize = 16;
    private const int TagSize = 16;
    private const int IterationCount = 100000;

    public EncryptionService()
    {
    }

    public string Encrypt(string plainText, string masterKey)
    {
        if (string.IsNullOrWhiteSpace(plainText))
        {
            throw new ArgumentException("Text cannot be null or empty", nameof(plainText));
        }

        if (string.IsNullOrWhiteSpace(masterKey))
        {
            throw new ArgumentException("Master key cannot be null or empty", nameof(masterKey));
        }

        var salt = new byte[SaltSize];
        var iv = new byte[IvSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
            rng.GetBytes(iv);
        }

        var derivedKey = DeriveKey(masterKey, salt);

        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        var ciphertext = new byte[plainTextBytes.Length];
        var tag = new byte[TagSize];

        using (var aes = new AesGcm(derivedKey, TagSize))
        {
            aes.Encrypt(iv, plainTextBytes, ciphertext, tag);
        }

        var result = new byte[SaltSize + IvSize + TagSize + ciphertext.Length];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(iv, 0, result, SaltSize, IvSize);
        Buffer.BlockCopy(tag, 0, result, SaltSize + IvSize, TagSize);
        Buffer.BlockCopy(ciphertext, 0, result, SaltSize + IvSize + TagSize, ciphertext.Length);

        return Convert.ToBase64String(result);
    }

    public async Task<string> EncryptAsync(string plainText, string masterKey)
    {
        return await Task.Run(() => Encrypt(plainText, masterKey));
    }

    public string Decrypt(string encryptedText, string masterKey)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
        {
            throw new ArgumentException("Encrypted text cannot be null or empty", nameof(encryptedText));
        }

        if (string.IsNullOrWhiteSpace(masterKey))
        {
            throw new ArgumentException("Master key cannot be null or empty", nameof(masterKey));
        }

        var encryptedData = Convert.FromBase64String(encryptedText);

        if (encryptedData.Length < SaltSize + IvSize + TagSize + 1)
        {
            throw new ArgumentException("Invalid encrypted text format");
        }

        var salt = new byte[SaltSize];
        var iv = new byte[IvSize];
        var tag = new byte[TagSize];
        var ciphertext = new byte[encryptedData.Length - SaltSize - IvSize - TagSize];

        Buffer.BlockCopy(encryptedData, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(encryptedData, SaltSize, iv, 0, IvSize);
        Buffer.BlockCopy(encryptedData, SaltSize + IvSize, tag, 0, TagSize);
        Buffer.BlockCopy(encryptedData, SaltSize + IvSize + TagSize, ciphertext, 0, ciphertext.Length);

        var derivedKey = DeriveKey(masterKey, salt);

        var plainTextBytes = new byte[ciphertext.Length];
        using (var aes = new AesGcm(derivedKey, TagSize))
        {
            aes.Decrypt(iv, ciphertext, tag, plainTextBytes);
        }

        var result = Encoding.UTF8.GetString(plainTextBytes);
        return result;
    }

    public async Task<string> DecryptAsync(string encryptedText, string masterKey)
    {
        return await Task.Run(() => Decrypt(encryptedText, masterKey));
    }

    private byte[] DeriveKey(string masterKey, byte[] salt)
    {
        var masterKeyBytes = Convert.FromBase64String(masterKey);
        using var pbkdf2 = new Rfc2898DeriveBytes(masterKeyBytes, salt, IterationCount, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(KeySize);
    }

    public string GenerateTemporaryPassword()
    {
        const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string numberChars = "0123456789";
        const string specialChars = "@$!%*?&";
        const int passwordLength = 8;

        var password = new List<char>
        {
            GetRandomChar(uppercaseChars),
            GetRandomChar(lowercaseChars),
            GetRandomChar(numberChars),
            GetRandomChar(specialChars)
        };

        var allChars = uppercaseChars + lowercaseChars + numberChars + specialChars;
        for (var i = 0; i < passwordLength - 4; i++)
        {
            password.Add(GetRandomChar(allChars));
        }

        var n = password.Count;
        while (n > 1)
        {
            var box = new byte[1];
            do
            {
                RandomNumberGenerator.Fill(box);
            }
            while (!(box[0] < n * (byte.MaxValue / n)));
            var k = box[0] % n;
            n--;
            var value = password[k];
            password[k] = password[n];
            password[n] = value;
        }

        return new string(password.ToArray());
    }

    private static char GetRandomChar(string chars)
    {
        var randomBytes = new byte[1];
        do
        {
            RandomNumberGenerator.Fill(randomBytes);
        }
        while (!(randomBytes[0] < chars.Length * (byte.MaxValue / chars.Length)));
        return chars[randomBytes[0] % chars.Length];
    }
}
