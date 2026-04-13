# Encryption Service Documentation

## Overview

The `EncryptionService` provides secure AES-GCM encryption/decryption capabilities for sensitive data in the iNestHRMS application. It uses industry-standard cryptographic algorithms to ensure data confidentiality and integrity.

## Features

- **AES-GCM Encryption**: Provides both confidentiality and authenticity
- **PBKDF2 Key Derivation**: Protects against brute-force attacks (100,000 iterations)
- **Random Salt & IV**: Unique encryption for each operation
- **Cryptographically Secure Random**: Uses `RandomNumberGenerator` instead of `System.Random`
- **Async Support**: Async methods for better performance

## Security Specifications

- **Encryption Algorithm**: AES-256-GCM
- **Key Size**: 256 bits (32 bytes)
- **IV Size**: 96 bits (12 bytes) - recommended for AES-GCM
- **Tag Size**: 128 bits (16 bytes) - authentication tag
- **Salt Size**: 128 bits (16 bytes)
- **KDF**: PBKDF2-HMAC-SHA256 with 100,000 iterations

## Setup

The `EncryptionService` is automatically registered in the dependency injection container via `SharedKernel/Startup.cs`.

```csharp
// Already configured in Startup.cs
services.AddScoped<IEncryptionService, EncryptionService>();
```

## Master Key Management

### Generating a Master Key

**IMPORTANT**: The master key should be:
- **32 bytes** (256 bits) of random data, Base64 encoded
- Stored securely in **Azure Key Vault**, **AWS Secrets Manager**, or similar
- **NEVER** hardcoded in the application

```csharp
// Generate a master key (do this once)
using var rng = RandomNumberGenerator.Create();
byte[] keyBytes = new byte[32];
rng.GetBytes(keyBytes);
string masterKey = Convert.ToBase64String(keyBytes);

// Store this in Azure Key Vault or appsettings (for development only)
```

### Storing the Master Key

#### Development (appsettings.json)
```json
{
  "Encryption": {
    "MasterKey": "YOUR_BASE64_ENCODED_KEY_HERE"
  }
}
```

#### Production (Azure Key Vault - Recommended)
```csharp
// Retrieve from Azure Key Vault
var keyVaultClient = new SecretClient(new Uri("https://your-vault.vault.azure.net/"), new DefaultAzureCredential());
KeyVaultSecret secret = await keyVaultClient.GetSecretAsync("EncryptionMasterKey");
string masterKey = secret.Value;
```

## Usage Examples

### 1. Basic Encryption/Decryption

```csharp
public class MyService
{
    private readonly IEncryptionService _encryptionService;
    private readonly string _masterKey; // Retrieved from configuration

    public MyService(IEncryptionService encryptionService, IConfiguration configuration)
    {
        _encryptionService = encryptionService;
        _masterKey = configuration["Encryption:MasterKey"];
    }

    public async Task<string> EncryptSensitiveData(string data)
    {
        return await _encryptionService.EncryptAsync(data, _masterKey);
    }

    public async Task<string> DecryptSensitiveData(string encryptedData)
    {
        return await _encryptionService.DecryptAsync(encryptedData, _masterKey);
    }
}
```

### 2. Encrypting Organization Data

```csharp
public class OrganizationService
{
    private readonly IEncryptionService _encryptionService;
    private readonly string _masterKey;

    public async Task<Organization> CreateOrganization(OrganizationDto dto)
    {
        var org = new Organization
        {
            CompanyName = dto.CompanyName,
            // Encrypt sensitive data
            EncryptedTaxId = await _encryptionService.EncryptAsync(dto.TaxId, _masterKey),
            EncryptedBankAccount = await _encryptionService.EncryptAsync(dto.BankAccount, _masterKey)
        };
        
        // Save to database
        return org;
    }

    public async Task<OrganizationDto> GetOrganization(Guid id)
    {
        var org = await _repository.GetByIdAsync(id);
        
        return new OrganizationDto
        {
            CompanyName = org.CompanyName,
            // Decrypt when retrieving
            TaxId = await _encryptionService.DecryptAsync(org.EncryptedTaxId, _masterKey),
            BankAccount = await _encryptionService.DecryptAsync(org.EncryptedBankAccount, _masterKey)
        };
    }
}
```

### 3. Password Reset with Temporary Password

```csharp
public class UserService
{
    private readonly IEncryptionService _encryptionService;
    private readonly string _masterKey;

    public async Task<string> GeneratePasswordResetToken(User user)
    {
        // Generate a secure temporary password
        string tempPassword = _encryptionService.GenerateTemporaryPassword();
        
        // Hash it for storage (using BCrypt)
        user.PasswordHash = PasswordHelper.HashPassword(tempPassword);
        
        // Encrypt it for the reset token
        string encryptedToken = await _encryptionService.EncryptAsync(tempPassword, _masterKey);
        
        // Send email with the reset link containing the encrypted token
        await SendPasswordResetEmail(user.Email, encryptedToken);
        
        return encryptedToken;
    }

    public async Task<bool> ResetPassword(string token, string newPassword)
    {
        // Decrypt the token to get the temporary password
        string tempPassword = await _encryptionService.DecryptAsync(token, _masterKey);
        
        // Verify and reset...
        return true;
    }
}
```

### 4. Encrypting User PII (Personally Identifiable Information)

```csharp
public class EmployeeService
{
    private readonly IEncryptionService _encryptionService;
    private readonly string _masterKey;

    public async Task<Employee> CreateEmployee(EmployeeDto dto)
    {
        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            // Encrypt PII
            EncryptedSSN = await _encryptionService.EncryptAsync(dto.SSN, _masterKey),
            EncryptedDateOfBirth = await _encryptionService.EncryptAsync(dto.DateOfBirth.ToString("yyyy-MM-dd"), _masterKey),
            EncryptedPersonalEmail = await _encryptionService.EncryptAsync(dto.PersonalEmail, _masterKey)
        };
        
        return employee;
    }
}
```

## Error Handling

The service throws exceptions for invalid inputs:

```csharp
try
{
    string encrypted = await _encryptionService.EncryptAsync(data, masterKey);
}
catch (ArgumentException ex)
{
    // Handle: empty plaintext, empty master key, or invalid encrypted format
    _logger.LogError(ex, "Encryption failed");
}
catch (CryptographicException ex)
{
    // Handle: decryption failed (wrong key, corrupted data, tampered data)
    _logger.LogError(ex, "Decryption failed - data may be corrupted or key is incorrect");
}
```

## Best Practices

1. **Always use async methods** in ASP.NET Core/Blazor applications
2. **Store master key securely** - never in source code
3. **Use different keys** for different environments (dev, staging, prod)
4. **Rotate keys periodically** (implement key versioning if needed)
5. **Log encryption/decryption failures** for security monitoring
6. **Validate data before encryption** to avoid encrypting empty strings
7. **Use HTTPS** to protect encrypted data in transit

## Password Generation

The service includes a secure temporary password generator:

```csharp
string tempPassword = _encryptionService.GenerateTemporaryPassword();
// Example output: "aB3$kL9x" (8 characters, includes uppercase, lowercase, number, special char)
```

## Migration from Existing Helper.GeneratePassword()

The old `Helper.GeneratePassword()` has been updated to use cryptographically secure random generation. However, `EncryptionService.GenerateTemporaryPassword()` is more feature-rich and should be preferred.

**Old (now updated)**:
```csharp
string password = Helper.GeneratePassword(); // Now uses RandomNumberGenerator
```

**New (recommended)**:
```csharp
string password = _encryptionService.GenerateTemporaryPassword(); // Same security, consistent API
```

## Performance Considerations

- Encryption/Decryption is CPU-intensive due to PBKDF2 (100K iterations)
- Use async methods to avoid blocking threads
- Consider caching decrypted data if read frequently (with appropriate security controls)
- Batch encrypt/decrypt operations when possible

## Security Audit Trail

Consider logging encryption/decryption operations for compliance:

```csharp
public async Task<string> EncryptWithAudit(string data, Guid userId)
{
    string encrypted = await _encryptionService.EncryptAsync(data, _masterKey);
    
    await _auditLog.LogAsync(new AuditEntry
    {
        UserId = userId,
        Action = "DataEncrypted",
        Timestamp = DateTime.UtcNow
    });
    
    return encrypted;
}
```

## Testing

```csharp
[Fact]
public async Task EncryptDecrypt_ShouldReturnOriginalText()
{
    // Arrange
    var service = new EncryptionService();
    string masterKey = GenerateTestMasterKey();
    string plainText = "Sensitive Data";

    // Act
    string encrypted = await service.EncryptAsync(plainText, masterKey);
    string decrypted = await service.DecryptAsync(encrypted, masterKey);

    // Assert
    Assert.Equal(plainText, decrypted);
    Assert.NotEqual(plainText, encrypted);
}
```

## Support

For questions or issues related to the Encryption Service, contact the development team or refer to the security documentation.
