# Encryption Service - Quick Start Guide

## ? Setup Complete!

The Encryption Service is now fully configured in your iNestHRMS application with the master key from your existing `SecretKey` configuration.

## ?? Configuration

### Master Key Location
The encryption master key is configured in:
- `src/services/WebApi/appsettings.json`
- `src/services/iNestHRMS_WebUI/appsettings.json`

```json
{
  "Encryption": {
    "MasterKey": "aU5FU1RIQY5NUzU3Q1JFVEtFWTk4NzY1NDMyMUFCQ0RFRkdISUdLTE1OTw=="
  }
}
```

**Note**: This is a Base64-encoded version derived from your existing SecretKey. In production, store this in Azure Key Vault!

## ?? How to Use

### Method 1: Using EncryptionHelper (Recommended - Easiest)

The `EncryptionHelper` automatically uses the master key from configuration:

```csharp
public class OrganizationController : ControllerBase
{
    private readonly EncryptionHelper _encryptionHelper;

    public OrganizationController(EncryptionHelper encryptionHelper)
    {
        _encryptionHelper = encryptionHelper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganization(OrganizationDto dto)
    {
        // Encrypt sensitive data
        var encryptedTaxId = await _encryptionHelper.EncryptAsync(dto.TaxId);
        var encryptedBankAccount = await _encryptionHelper.EncryptAsync(dto.BankAccount);

        // Save to database
        var org = new Organization
        {
            CompanyName = dto.CompanyName,
            EncryptedTaxId = encryptedTaxId,
            EncryptedBankAccount = encryptedBankAccount
        };

        await _repository.SaveAsync(org);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrganization(Guid id)
    {
        var org = await _repository.GetByIdAsync(id);

        // Decrypt sensitive data
        var dto = new OrganizationDto
        {
            CompanyName = org.CompanyName,
            TaxId = await _encryptionHelper.DecryptAsync(org.EncryptedTaxId),
            BankAccount = await _encryptionHelper.DecryptAsync(org.EncryptedBankAccount)
        };

        return Ok(dto);
    }
}
```

### Method 2: Using IEncryptionService (Advanced)

If you need more control or want to use a different master key:

```csharp
public class MyService
{
    private readonly IEncryptionService _encryptionService;
    private readonly EncryptionSettings _settings;

    public MyService(IEncryptionService encryptionService, EncryptionSettings settings)
    {
        _encryptionService = encryptionService;
        _settings = settings;
    }

    public async Task<string> EncryptData(string data)
    {
        return await _encryptionService.EncryptAsync(data, _settings.MasterKey);
    }
}
```

## ?? Common Use Cases

### 1. Encrypt Organization Tax ID and Bank Account

```csharp
// Before saving
org.EncryptedTaxId = await _encryptionHelper.EncryptAsync(dto.TaxId);
org.EncryptedBankAccount = await _encryptionHelper.EncryptAsync(dto.BankAccount);

// When retrieving
dto.TaxId = await _encryptionHelper.DecryptAsync(org.EncryptedTaxId);
dto.BankAccount = await _encryptionHelper.DecryptAsync(org.EncryptedBankAccount);
```

### 2. Generate Temporary Password for Password Reset

```csharp
// Generate and encrypt
var (plainPassword, encryptedPassword) = await _encryptionHelper.GenerateAndEncryptPassword();

// Send plainPassword to user via email
await _emailService.SendPasswordResetEmail(user.Email, plainPassword);

// Store encryptedPassword in database or reset token
user.PasswordResetToken = encryptedPassword;
```

### 3. Encrypt Employee SSN and Personal Data

```csharp
employee.EncryptedSSN = await _encryptionHelper.EncryptAsync(dto.SSN);
employee.EncryptedPersonalEmail = await _encryptionHelper.EncryptAsync(dto.PersonalEmail);
employee.EncryptedDateOfBirth = await _encryptionHelper.EncryptAsync(dto.DateOfBirth.ToString("yyyy-MM-dd"));
```

## ?? Database Schema Changes

Update your database tables to store encrypted data:

```sql
-- For Organization table
ALTER TABLE Organizations 
ADD EncryptedTaxId NVARCHAR(MAX) NULL,
    EncryptedBankAccount NVARCHAR(MAX) NULL;

-- For Employee table
ALTER TABLE Employees 
ADD EncryptedSSN NVARCHAR(MAX) NULL,
    EncryptedPersonalEmail NVARCHAR(MAX) NULL,
    EncryptedDateOfBirth NVARCHAR(MAX) NULL;
```

**Important**: Encrypted data is Base64-encoded and will be longer than the original data. Use `NVARCHAR(MAX)` or a large enough size.

## ?? What's Encrypted?

Typically, you should encrypt:
- ? Social Security Numbers (SSN)
- ? Tax IDs (TIN, EIN, GSTIN)
- ? Bank Account Numbers
- ? Personal Email Addresses
- ? Dates of Birth
- ? Passport Numbers
- ? Credit Card Numbers
- ? Any other PII (Personally Identifiable Information)

**Don't encrypt**:
- ? Passwords (use BCrypt hashing instead - already implemented in `PasswordHelper`)
- ? Data used for searching/indexing
- ? Non-sensitive public information

## ??? Integration Points

### 1. In Your Organization Feature

```csharp
// File: src/services/WebApi/Features/Organization/Infrastructure/OrganizationRepository.cs

public class OrganizationService
{
    private readonly EncryptionHelper _encryptionHelper;
    private readonly OrganizationRepository _repository;

    public async Task<Guid> CreateOrganization(RequestOrganizationDTOs dto)
    {
        // Encrypt GSTIN if provided
        if (!string.IsNullOrEmpty(dto.GSTIN))
        {
            dto.GSTIN = await _encryptionHelper.EncryptAsync(dto.GSTIN);
        }

        return await _repository.UpsertOrganizationAsync(dto);
    }

    public async Task<ResponseOrganizationDTOs> GetOrganization(Guid id)
    {
        var org = await _repository.GetByIdAsync(id);

        // Decrypt GSTIN
        if (!string.IsNullOrEmpty(org.GSTIN))
        {
            org.GSTIN = await _encryptionHelper.DecryptAsync(org.GSTIN);
        }

        return org;
    }
}
```

### 2. In Your Auth Feature (Password Reset)

```csharp
public class AuthService
{
    private readonly EncryptionHelper _encryptionHelper;

    public async Task<string> InitiatePasswordReset(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        // Generate secure temporary password
        var tempPassword = _encryptionHelper.GenerateTemporaryPassword();

        // Hash it for database storage
        user.PasswordHash = PasswordHelper.HashPassword(tempPassword);

        // Encrypt it for the reset token/email
        var encryptedToken = await _encryptionHelper.EncryptAsync(tempPassword);

        // Send email with temporary password
        await _emailService.SendPasswordResetEmail(email, tempPassword);

        return encryptedToken;
    }
}
```

## ?? Testing

```csharp
[Fact]
public async Task CanEncryptAndDecrypt()
{
    // Arrange
    var encryptionHelper = _serviceProvider.GetRequiredService<EncryptionHelper>();
    string original = "Sensitive Data 123";

    // Act
    string encrypted = await encryptionHelper.EncryptAsync(original);
    string decrypted = await encryptionHelper.DecryptAsync(encrypted);

    // Assert
    Assert.NotEqual(original, encrypted);
    Assert.Equal(original, decrypted);
}
```

## ?? Important Security Notes

1. **Master Key Storage**:
   - ? Development: OK to use appsettings.json
   - ? Production: MUST use Azure Key Vault or similar
   - ? NEVER commit production keys to source control

2. **Key Rotation**:
   - Plan for key rotation (store key version with encrypted data)
   - Keep old keys to decrypt existing data

3. **Error Handling**:
   ```csharp
   try
   {
       var decrypted = await _encryptionHelper.DecryptAsync(encrypted);
   }
   catch (CryptographicException ex)
   {
       _logger.LogError(ex, "Decryption failed - data may be corrupted");
       // Handle error appropriately
   }
   ```

## ?? More Examples

See detailed examples in:
- `src/services/SharedKernel/Examples/EncryptionServiceExamples.cs`
- `src/services/WebApi/Examples/EncryptionUsageExamples.cs`
- `docs/EncryptionService.md`

## ?? You're Ready!

The encryption service is now ready to use. Simply inject `EncryptionHelper` into any service or controller and start encrypting sensitive data!

```csharp
public MyController(EncryptionHelper encryptionHelper)
{
    _encryptionHelper = encryptionHelper;
}
```

That's it! ??
